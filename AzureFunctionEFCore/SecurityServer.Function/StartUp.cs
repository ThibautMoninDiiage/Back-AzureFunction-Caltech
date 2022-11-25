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

[assembly: FunctionsStartup(typeof(StartUp))]

namespace SecurityServer.Function
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);
            // configuration de la configuration relative à l'API dans l'API, fortement typée
            //builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            // Utilisation de la configuration fortement typée dans le Program.cs
            var conf = new ApiSettings();
            //builder.Configuration.GetSection(nameof(ApiSettings)).Bind(conf);

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
                    ValidIssuer = conf.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = conf.JwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf.JwtSecret))
                };
            });
            builder.Services.AddDbContext<DbContextServer>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
            builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
        }
    }
}
