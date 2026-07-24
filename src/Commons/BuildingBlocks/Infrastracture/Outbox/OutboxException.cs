namespace BuildingBlocks.Infrastracture.Outbox
{
    public class OutboxException : Exception
    {
        public OutboxException(string message, Exception? inner) : base(message, inner)
        {
        }
    }
}
