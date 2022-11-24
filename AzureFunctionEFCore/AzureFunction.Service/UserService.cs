using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.DTO.Down;
using SecurityServer.Models.Models;
using SecurityServer.Service.Interfaces;
using System.Linq.Expressions;

namespace SecurityServer.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User?> Get(string login, string password)
        {
            var type = typeof(User);
            var member = Expression.Parameter(type, "param");
            var memberLogin = Expression.PropertyOrField(member, "username");
            var memberPassword = Expression.PropertyOrField(member, "password");
            var requete = Expression.Lambda<Func<User, bool>>(Expression.AndAlso(Expression.Equal(memberLogin, Expression.Constant(login)), Expression.Equal(memberPassword, Expression.Constant(password))), member);
            return await _uow.UserRepository.GetAsync(requete) ?? null;
        }

        public async Task<UserDtoDown?> GetById(int? id)
        {
            var type = typeof(User);
            var member = Expression.Parameter(type, "param");
            var memberId = Expression.PropertyOrField(member, "id");
            var requete = Expression.Lambda<Func<User, bool>>(Expression.Equal(memberId, Expression.Constant(id)), member);

            User user = await _uow.UserRepository.GetAsync(requete);

            if (user != null)
            {
                UserDtoDown userDtoDown = new UserDtoDown()
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    Mail = user.Mail,
                    Username = user.Username
                };
                return userDtoDown;
            }
            else
                return null;
        }
    }
}