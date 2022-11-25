using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
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
            Type type = typeof(User);
            ParameterExpression member = Expression.Parameter(type, "param");
            MemberExpression fieldLogin = Expression.PropertyOrField(member, "username");
            MemberExpression fieldPassword = Expression.PropertyOrField(member, "password");
            Expression<Func<User,bool>> requete = Expression.Lambda<Func<User, bool>>(Expression.AndAlso(Expression.Equal(fieldLogin, Expression.Constant(login)), Expression.Equal(fieldPassword, Expression.Constant(password))), member);
            return await _uow.UserRepository.GetAsync(requete) ?? null;
        }

        public async Task<UserDtoDown?> GetById(int? id)
        {
            Type type = typeof(User);
            ParameterExpression member = Expression.Parameter(type, "param");
            MemberExpression fieldId = Expression.PropertyOrField(member, "id");
            Expression<Func<User, bool>> requete = Expression.Lambda<Func<User, bool>>(Expression.Equal(fieldId, Expression.Constant(id)), member);

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