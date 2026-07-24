using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BuildingBlocks.Infrastracture.Outbox
{
    public sealed class OutboxProcessor<TContext>(
        TContext context,
        IPublishEndpoint publisher,
        ILogger<OutboxProcessor<TContext>> logger)
        : IOutboxProcessor
        where TContext : DbContext
    {
        private const int BatchSize = 100;

        public async Task ProcessMessageAsync(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Beginning to process outbox messages");
            var affectedRows = 0;
            var unProcessedMessages = await context
                .Set<OutboxMessage>()
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccuredOnUtc)
                .Take(BatchSize)
                .ToListAsync();
            if (!unProcessedMessages.Any())
            {
                logger.LogInformation("Completed processing outbox messages - no messages to process");
                return;
            }
            foreach (var unprocessedMessage in unProcessedMessages)
            {
                logger.LogInformation("Starting process message {id} ", unprocessedMessage);

                var messageType = Type.GetType(unprocessedMessage.Type);

                try
                {
                    var deserializedMessage =
                       JsonSerializer.Deserialize(unprocessedMessage.Content, messageType)
                          ?? throw new JsonException($"Deserialization returned null for message " +
                           $"'{unprocessedMessage.Id}'.");

                    await publisher.Publish(
                       message: deserializedMessage,
                       messageType: messageType,
                       publishContext =>
                       {
                           publishContext.MessageId = unprocessedMessage.Id;
                       },
                       cancellationToken: cancellationToken);

                    affectedRows += await context
                         .Set<OutboxMessage>()
                         .Where(x => x.Id == unprocessedMessage.Id)
                         .ExecuteUpdateAsync(setters =>
                             setters
                             .SetProperty(p => p.ProcessedOnUtc, DateTime.UtcNow)
                             .SetProperty(p => p.Error, (string?)null), cancellationToken);
                }
                catch (Exception ex)
                {
                    await context.Set<OutboxMessage>()
                         .Where(x => x.Id == unprocessedMessage.Id)
                         .ExecuteUpdateAsync(setters =>
                             setters
                             .SetProperty(p => p.Error, ex.ToString()), cancellationToken);
                    logger.LogError(
                        ex,
                        "Exception while processing outbox message {id}",
                        unprocessedMessage.Id);
                }
            }

            logger.LogInformation("Completed processing outbox messages");
        }
    }
}
