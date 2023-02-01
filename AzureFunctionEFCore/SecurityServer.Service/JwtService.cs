using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using SecurityServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SecurityServer.Service.Interfaces;
using Claim = System.Security.Claims.Claim;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace SecurityServer.Service
{
    public class JwtService : IJwtService
    {

        private readonly ApiSettings _apiSettings;

        public JwtService(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public static X509Certificate2 LoadCertificate(string vaultUrl,string clientId,string tenantId,string secret)
        {
            var certificateName = "certificatetoken";

            var credentials = new ClientSecretCredential(tenantId,clientId,secret);
            var certClient = new CertificateClient(new Uri(vaultUrl), credentials);
            var secretClient = new SecretClient(new Uri(vaultUrl),credentials);


            KeyVaultCertificateWithPolicy certificate = certClient.GetCertificate(certificateName);
            if (certificate.Policy?.Exportable != true)
            {
                return new X509Certificate2(certificate.Cer);
            }

            string[] segments = certificate.SecretId.AbsolutePath.Split('/',StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length != 3)
                throw new Exception("Le certificat n'est pas complet");

            string secretName = segments[1];
            string secretVersion = segments[2];

            KeyVaultSecret keyVaultSecret = secretClient.GetSecret(secretName,secretVersion);

            if ("application/x-pkcs12".Equals(keyVaultSecret.Properties.ContentType, StringComparison.InvariantCultureIgnoreCase))
            {
                byte[] pfx = Convert.FromBase64String(keyVaultSecret.Value);
                return new X509Certificate2(pfx);
            }

            throw new NotSupportedException("Le certificat n'est pas au format application/x-pkcs12");


        }

        public string GenerateJwtToken(int idUser,int idRole)
        {

            X509Certificate2 certificate = LoadCertificate("https://preprodkeyvaultgdeuxb.vault.azure.net/", "4e9414cf-0bfd-4144-880c-ccff9e466553", "14bc5219-40ca-4d62-a8e4-7c97c1236349", "woJ8Q~UaQLITEXeUaiyKoy1mOGTplvEj8K5WObS2");

            RSA test = certificate.GetRSAPrivateKey();
            RsaSecurityKey securityKey = new RsaSecurityKey(test);


            // génère un token valide pour 7 jours
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", idUser.ToString()),
                    new Claim("idRole", idRole.ToString()),
                    // Cela va garantir que le token est unique
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Issuer = _apiSettings.JwtIssuer,
                Audience = _apiSettings.JwtAudience,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt;
        }

        public string HashPasswordWithSalt(string password, byte[] salt)
        {
            // obtenir une clé de 256-bit (en utilisant HMACSHA256 sur 100,000 itérations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public Guid GenerateGrantCode()
        {
            return Guid.NewGuid();
        }
    }
}
