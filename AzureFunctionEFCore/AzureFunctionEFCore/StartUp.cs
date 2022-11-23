using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using AzureFunction.Service.Interfaces;
using AzureFunction.Service;
using AzureFunction.Contract.UnitOfWork;
using AzureFunction.UnitOfWork.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using AzureFunction.UnitOfWork.DbContextAZ;
using Microsoft.IdentityModel.Protocols;

[assembly: FunctionsStartup(typeof(AzureFunctionEFCore.StartUp))]

namespace AzureFunctionEFCore
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);
            builder.Services.AddDbContext<DbContextServeur>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
        }
    }
}
