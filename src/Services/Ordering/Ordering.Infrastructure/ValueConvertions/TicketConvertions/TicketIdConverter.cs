using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Infrastructure.ValueConvertions.TicketConvertions
{
    internal sealed class TicketIdConverter : ValueConverter<TicketId,Guid>
    {
        public TicketIdConverter() : 
            base(
                id => id.Value , 
                value => new TicketId(value))
        { }
    }
}
