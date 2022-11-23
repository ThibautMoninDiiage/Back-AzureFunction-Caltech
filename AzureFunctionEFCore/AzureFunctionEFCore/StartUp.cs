using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using AzureFunction.Service.Interfaces;
using AzureFunction.Service;
using AzureFunction.Contract.UnitOfWork;
using AzureFunction.UnitOfWork.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using AzureFunction.UnitOfWork.DbContextAZ;

[assembly: FunctionsStartup(typeof(AzureFunctionEFCore.StartUp))]

namespace AzureFunctionEFCore
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = "Server=tcp:test-server-database.database.windows.net,1433;Initial Catalog=testdbdiiagedeux;Persist Security Info=False;User ID=CloudSAf42aaec6;Password=Azerty@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            builder.Services.AddDbContext<DbContextServeur>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
        }
    }
}
