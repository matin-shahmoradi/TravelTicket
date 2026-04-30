namespace Basket.API.StrongIdTypes
{
    [StronglyTypedId]
    public partial struct ShoppingCartId()
    {
        public static ShoppingCartId New(Guid Value) => new ShoppingCartId(Value);
    }
}
