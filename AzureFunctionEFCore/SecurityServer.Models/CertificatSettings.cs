namespace SecurityServer.Models
{
    public class CertificatSettings
    {
        public string? VaultUrl { get; set; }
        public string? ClientId { get; set; }
        public string? TenantId { get; set; }
        public string? Secret { get; set; }
        public string? CertificateName { get; set; }
    }
}
