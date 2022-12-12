using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SecurityServer.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly ApiSettings _apiSettings;
        public UserService(IUnitOfWork uow, IOptions<ApiSettings> apiSettings)
        {
            _uow = uow;
            this._apiSettings = apiSettings.Value;
        }

        public async Task<User?> GetById(int? id)
        {
            Type type = typeof(User);
            ParameterExpression member = Expression.Parameter(type, "param");
            MemberExpression fieldId = Expression.PropertyOrField(member, "id");
            Expression<Func<User, bool>> requete = Expression.Lambda<Func<User, bool>>(Expression.Equal(fieldId, Expression.Constant(id)), member);

            User user = await _uow.UserRepository.GetAsync(requete);

            if (user != null)
                return user;
            else
                return null;
        }

        public async Task<UserDtoDown> Authenticate(UserDtoUp model)
        {

            Type type = typeof(User);
            ParameterExpression member = Expression.Parameter(type, "param");
            MemberExpression fieldLogin = Expression.PropertyOrField(member, "username");
            Expression<Func<User, bool>> requete = Expression.Lambda<Func<User, bool>>(Expression.Equal(fieldLogin, Expression.Constant(model.UserName)), member);

            User user = await _uow.UserRepository.GetAsync(requete);

            // return null si on ne trouve pas l'utilisateur
            if (user == null)
                return null;

            var hashedPassword = HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            // Si les mots de passe ne correspondent pas on retourne null.
            if (user.Password != hashedPassword)
                return null;

            // authentification réussie, on génère le token
            var token = generateJwtToken(user);

            return new UserDtoDown(user, token);
        }

        public async Task<UserDtoDown> CreateUser(UserCreationDtoUp model)
        {
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(model.Password, salt);

            var user = new User
            {
                Mail = model.Mail,
                Username = model.Username,
                Password = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IdRole = 2,
                Salt = Convert.ToBase64String(salt)
            };

            _uow.UserRepository.Add(user);
            await _uow.CommitAsync();

            var token = generateJwtToken(user);
            return new UserDtoDown(user, token);
        }

        private string generateJwtToken(User user)
        {
            // génère un token valide pour 7 jours
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_apiSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Mail),
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

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt;
        }

        private string HashPasswordWithSalt(string password, byte[] salt)
        {
            // obtenir une clé de 256-bit (en utilisant HMACSHA256 sur 100,000 itérations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}