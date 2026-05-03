using Basket.API.StrongIdTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Basket.API.Data.ValueConverters
{
    public class ShoppingCartItemIdConverter : ValueConverter<ShoppingCartItemId, Guid>
    {
        public ShoppingCartItemIdConverter() : base
            (
                id => id.Value,
                value => ShoppingCartItemId.New()
            )
        {
            
        }
    }
}
