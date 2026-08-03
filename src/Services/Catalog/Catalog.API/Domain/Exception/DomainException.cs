namespace Catalog.API.Domain.Exception
{
    [Serializable]
    public class DomainException : System.Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, System.Exception inner) : base(message, inner) { }
    }
}
