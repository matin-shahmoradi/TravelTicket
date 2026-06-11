namespace AuthService.OptionProperties
{
    public class FluentEmailOptions
    {
        public string SenderEmail { get; set; } = default!;
        public string Sender { get; set; } = default!;
        public string Host { get; set; } = default!;
        public string Port { get; set; } = default!;
    }
}
