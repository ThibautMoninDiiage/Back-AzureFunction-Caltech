namespace SecurityServer.Models
{
    public class ApiSettings
    {
        public string? JwtSecret { get; set; }
        public string? JwtIssuer { get; set; }
        public string? JwtAudience { get; set; }
    }
}
