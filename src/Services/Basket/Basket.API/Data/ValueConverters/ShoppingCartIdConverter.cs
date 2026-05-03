using Basket.API.StrongIdTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Basket.API.Data.ValueConverters
{
    public class ShoppingCartIdConverter : ValueConverter<ShoppingCartId , Guid>
    {
        public ShoppingCartIdConverter() : base
            (
                id => id.Value,
                value => ShoppingCartId.New()
            )
        {
            
        }
    }
}
