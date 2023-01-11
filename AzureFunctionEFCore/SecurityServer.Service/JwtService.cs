using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using SecurityServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SecurityServer.Service.Interfaces;
using Claim = System.Security.Claims.Claim;

namespace SecurityServer.Service
{
    public class JwtService : IJwtService
    {

        private readonly ApiSettings _apiSettings;

        public JwtService(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public string generateJwtToken(int idUser,int idRole)
        {
            // génère un token valide pour 7 jours
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_apiSettings.JwtSecret);
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
