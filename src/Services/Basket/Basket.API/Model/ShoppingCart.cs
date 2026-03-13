namespace Basket.API.Model
{
    public class ShoppingCart
    {  
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<ShoppingCartItem> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
        public string TravlerNumber { get; set; } = default!;

        public ShoppingCart()
        {
            
        }
        public ShoppingCart(string travlerNumber)
        {
            TravlerNumber = travlerNumber;
        }
    }
}
