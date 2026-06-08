using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BuildingBlocks.Infrastracture.CorrelationId
{
    public sealed class CorrelationIdMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[CorrelationConstants.HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(correlationId))
                correlationId = Guid.NewGuid().ToString();

            context.Items[CorrelationConstants.HeaderName] = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty(CorrelationConstants.LogPropertyName, correlationId))
            {
                await next(context);
            }
        }
    }
}
