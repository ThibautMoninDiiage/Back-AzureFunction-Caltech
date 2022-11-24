using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using SecurityServer.Service;
using SecurityServer.Service.Interfaces;
using SecurityServer.Function;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.DataAccess.DbContextAZ;
using SecurityServer.DataAccess.UnitOfWork;

[assembly: FunctionsStartup(typeof(StartUp))]

namespace SecurityServer.Function
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);
            builder.Services.AddDbContext<DbContextServeur>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
            builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
        }
    }
}
