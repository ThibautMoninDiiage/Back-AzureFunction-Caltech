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

            builder.Services.AddHealthChecks();
            builder.Services.AddCors(options => options.AddPolicy(
                "CorsPolicy",builder => builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            builder.Services.AddEndpointsApiExplorer();

            var jwt = new ApiSettings();

            jwt.JwtSecret = Environment.GetEnvironmentVariable("JwtSecret", EnvironmentVariableTarget.Process);
            jwt.JwtIssuer = Environment.GetEnvironmentVariable("JwtIssuer", EnvironmentVariableTarget.Process);
            jwt.JwtAudience = Environment.GetEnvironmentVariable("JwtAudience", EnvironmentVariableTarget.Process);

            var certificat = new CertificatSettings();

            certificat.VaultUrl = Environment.GetEnvironmentVariable("Vault_url", EnvironmentVariableTarget.Process);
            certificat.Secret = Environment.GetEnvironmentVariable("Secret", EnvironmentVariableTarget.Process);
            certificat.ClientId = Environment.GetEnvironmentVariable("Client_id", EnvironmentVariableTarget.Process);
            certificat.TenantId = Environment.GetEnvironmentVariable("Tenant_id", EnvironmentVariableTarget.Process);
            certificat.CertificateName = Environment.GetEnvironmentVariable("CertificateName", EnvironmentVariableTarget.Process);

            builder.Services.AddSingleton(jwt);
            builder.Services.AddSingleton(certificat);

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
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IApplicationService,ApplicationService>();
            builder.Services.AddTransient<IAdminService, AdminService>();
            builder.Services.AddTransient<IJwtService, JwtService>();
        }
    }
}
