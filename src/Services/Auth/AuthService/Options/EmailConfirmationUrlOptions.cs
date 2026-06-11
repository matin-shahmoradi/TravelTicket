namespace AuthService.Options
{
    public class EmailConfirmationUrlOptions
    {
        public string httpUrl { get; set; } = default!;
        public string httpsUrl { get; set; } = default!;
    }
}
