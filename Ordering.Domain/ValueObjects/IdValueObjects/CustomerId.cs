namespace Ordering.Domain.ValueObjects.IdValueObjects
{
    public readonly record struct CustomerId(Guid Value)
    {
        public static CustomerId New(Guid value) => new CustomerId(value);
        public static CustomerId New() => new CustomerId(Guid.NewGuid());
    }
}
