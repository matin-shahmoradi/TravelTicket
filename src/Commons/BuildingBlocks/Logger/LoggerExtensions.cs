namespace BuildingBlocks.Logger
{
    public static class LoggerExtensions
    {
        public static void LogEventHandler(
            this ILogger logger,
            string eventName)
        {
            logger.LogInformation("[EVENT HANDLER] {eventName} Handled successfully", eventName);
        }
        public static void LogEventReceived(
            this ILogger logger,
            Guid? messageId,
            string consumerName)
        {
            logger.LogInformation(
                "[INTEGRATION EVENT] RECEIVED | MessageId : {messageId}, Consumer : {consumerName}",
                messageId,
                consumerName);
        }

        public static void LogEventConsumed(
            this ILogger logger,
            Guid? messageId,
            string consumerName)
        {
            logger.LogInformation(
               "[EVENT CONSUMER] | MessageId : {messageId} consumed by : {consumerName}",
               messageId,
               consumerName);
        }
    }
}
