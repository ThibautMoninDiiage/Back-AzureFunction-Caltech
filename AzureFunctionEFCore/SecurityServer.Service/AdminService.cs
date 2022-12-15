using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;
using System.Linq;
using System.Security.Cryptography;

namespace SecurityServer.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _uow;

        public AdminService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User> CreateUser(UserCreationDtoUp model)
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
                Avatar = model.Avatar,
                Salt = Convert.ToBase64String(salt)
            };

            if (string.IsNullOrEmpty(model.Role))
                user.IdRole = 2;
            else
                user.IdRole = 1;

            _uow.UserRepository.Add(user);
            await _uow.CommitAsync();

            return user;
        }

        public async Task<List<UserAllDtoDown>> GetAllUsers()
        {

            List<User> users = _uow.UserRepository.GetAllAsync().Result.ToList();
            List<UserAllDtoDown> userAllDtoDowns = new List<UserAllDtoDown>();

            users.ForEach(u => userAllDtoDowns.Add(new UserAllDtoDown() { Id = u.Id, Avatar = u.Avatar, Mail = u.Mail, Username = u.Username,Role = new RoleUserDtoDown() { Name = GetRoleById((int)u.IdRole).Name} }));

            return userAllDtoDowns;
        }

        public Role GetRoleById(int id)
        {
            return _uow.RoleRepository.Get(x => x.Id == id);
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
