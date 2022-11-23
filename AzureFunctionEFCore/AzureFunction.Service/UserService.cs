using AzureFunction.Contract.UnitOfWork;
using AzureFunction.Models;
using AzureFunction.Service.Interfaces;
using System.Linq.Expressions;

namespace AzureFunction.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<User> Get(string login, string password)
        {
            var type = typeof(User);
            var member = Expression.Parameter(type, "param");
            var memberLogin = Expression.PropertyOrField(member, "username");
            var memberPassword = Expression.PropertyOrField(member, "password");
            //var targetMethodLogin = memberLogin.Type.GetMethod("Find", new Type[] { typeof(string), typeof(StringComparison) });
            //var targetMethodPassword = memberPassword.Type.GetMethod("Find", new Type[] { typeof(string), typeof(StringComparison) });
            //var methodCallExpressionLogin = Expression.Call(memberLogin, targetMethodLogin, Expression.Constant(login), Expression.Constant(StringComparison.CurrentCultureIgnoreCase));
            //var methodCallExpressionPassword = Expression.Call(memberPassword, targetMethodPassword, Expression.Constant(password), Expression.Constant(StringComparison.CurrentCultureIgnoreCase));

            var t = Expression.Lambda<Func<User, bool>>(
            Expression.AndAlso(
                Expression.Equal(memberLogin, Expression.Constant(login)),
                Expression.Equal(memberPassword, Expression.Constant(password))
            ),
            member
        );

            return _uow.UserRepository.Get(t);
            //throw new NotImplementedException();
        }
    }
}