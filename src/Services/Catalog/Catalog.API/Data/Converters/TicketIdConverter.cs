using Catalog.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.API.Data.Converters
{
    public class TicketIdConverter : ValueConverter<TicketId,Guid>
    {
        public TicketIdConverter() : base
            (
                ticketId => ticketId.Value,
                value => TicketId.New(value)
            )
        {
            
        }
    }
}
