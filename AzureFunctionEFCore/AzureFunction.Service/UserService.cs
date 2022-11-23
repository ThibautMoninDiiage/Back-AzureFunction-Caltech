using AzureFunction.Contract.UnitOfWork;
using AzureFunction.Models;
using AzureFunction.Service.Interfaces;

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
            //return await _uow.UserRepository.Get();
            throw new NotImplementedException();
        }
    }
}