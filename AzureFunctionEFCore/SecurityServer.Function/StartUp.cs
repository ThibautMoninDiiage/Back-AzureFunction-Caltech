using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using SecurityServer.Service;
using SecurityServer.Service.Interfaces;
using SecurityServer.Function;
using SecurityServer.DataAccess.SecurityServerContext;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.DataAccess.UnitOfWork;
using SecurityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

[assembly: FunctionsStartup(typeof(StartUp))]

namespace SecurityServer.Function
{
    public class StartUp : FunctionsStartup
    {

        private IConfiguration _configuration = null;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);

            builder.Services.AddEndpointsApiExplorer();

            IConfiguration localConfig = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

            ApiSettings jwt = localConfig.GetSection("ApiSettings").Get<ApiSettings>();
            //register with container
            builder.Services.AddSingleton(jwt);

            // configuration du middleware d'authentification JWT fourni par Microsoft
            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.JwtSecret))
                };
            })
            .AddCookie();

            builder.Services.AddDbContext<DbContextServer>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
            builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
        }
    }
}
