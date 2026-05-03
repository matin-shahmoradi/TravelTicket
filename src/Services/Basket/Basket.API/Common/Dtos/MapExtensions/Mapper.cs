namespace Basket.API.Common.Dtos.MapExtensions
{
    public static class Mapper
    {
        public static ShoppingCartDto MapToShoppingCartDto(this ShoppingCart shoppingCart)
        {
            return new ShoppingCartDto
            {
                Id = shoppingCart.Id.Value,
                Username = shoppingCart.Username,
                Items = shoppingCart.Items.Select(i => new ShoppingCartItemDto
                {
                    TicketId = i.TicketId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                TotalPrice = shoppingCart.TotalPrice
            };
        }

        public static ShoppingCart MapToShoppingCartEntity(this ShoppingCartDto dto)
        {
            var cart = ShoppingCart.Create(dto.Username);
            foreach (var item in dto.Items)
            {
                cart.AddItem(item.TicketId, item.Quantity, item.Price);
            }
            return cart;
        }
    }
}
