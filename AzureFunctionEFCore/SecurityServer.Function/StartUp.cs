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

[assembly: FunctionsStartup(typeof(StartUp))]

namespace SecurityServer.Function
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connString = Environment.GetEnvironmentVariable("SqlConnectionString", EnvironmentVariableTarget.Process);

            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen(option =>
            //{
            //    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //    {
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Description = "Ent�tes d'autorisation JWT \r\n\r\n Tapez 'Bearer' [espace] et votre token dans l'input qui suis.\r\n\r\nExemple: \"Bearer 1safsfsdfdfd\"",
            //    });

            //    option.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //           new OpenApiSecurityScheme
            //             {
            //                 Reference = new OpenApiReference
            //                 {
            //                     Type = ReferenceType.SecurityScheme,
            //                     Id = "Bearer"
            //                 }
            //             },
            //             new string[] {}
            //        }
            //    });
            //});

            // Utilisation de la configuration fortement typée dans le Program.cs
            var conf = new ApiSettings();

            builder.Services.AddOptions<ApiSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("ApiSettings").Bind(settings);
            });

            builder.Services.AddCors();

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
            })
            .AddCookie();

            builder.Services.AddDbContext<DbContextServer>(options => options.UseSqlServer(connString));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddTransient(typeof(IRoleService), typeof(RoleService));
            builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
            builder.Services.AddTransient(typeof(IApplicationService), typeof(ApplicationService));
        }
    }
}
