using AzureFunctionEFCore.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(AzureFunctionEFCore.StartUp))]

namespace AzureFunctionEFCore
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<DbContextEFCore>(options => SqlServerDbContextOptionsExtensions.UseSqlServer(options,connString));
        }
    }
}
