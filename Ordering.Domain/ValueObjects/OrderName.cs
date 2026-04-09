namespace Ordering.Domain.ValueObjects
{
    public readonly record struct OrderName(string Value)
    {
        public OrderName() : this(string.Empty)
        {

        }

        public static OrderName From(string value)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentNullException("Order name cannot be empty.",nameof(value));

            return new OrderName(value);

        }
        public override string ToString() => Value;
    }
}
