namespace BuildingBlocks.Messaging
{
    public interface IIntegrationEvent
    {
        public Guid EventId { get; }
        public DateTime OccuredOn { get; }
        public string EventType { get; }
    };

}
