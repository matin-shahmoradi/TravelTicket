using Ordering.Domain.ValueObjects.IdValueObjects;

namespace Ordering.Domain.Models
{
    public class Customer : Entity<CustomerId>
    {
        public string Name { get; private set; } = default!;
        public string NationalCode { get; private set; } = default!;
        public string PhoneNumber { get; private set; } = default!;
        public string? Email { get; private set; } = default!;

        public static Customer Create(CustomerId id, string name, string nationalCode, string phoneNumber, string email)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nationalCode);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(phoneNumber); 
            ArgumentNullException.ThrowIfNullOrWhiteSpace(email);

            var customer = new Customer
            {
                Id = id,
                Name = name,
                NationalCode = nationalCode,
                PhoneNumber = phoneNumber,
                Email = email
            };
            return customer;
        }
    }
}
