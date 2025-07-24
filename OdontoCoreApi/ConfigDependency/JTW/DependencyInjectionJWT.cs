using Gbarber.Application;
using GBarber.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GBarber.WebApi.ConfigDependency.JTW
{
    public static class DependencyInjectionJWT
    {
        public static IServiceCollection AddApplicationServicesJwt(this IServiceCollection services,WebApplicationBuilder builder)
        {

            services.Configure<JWT>(builder.Configuration.GetSection("jwtConfig"));
            var jwtConfig = builder.Configuration.GetSection("jwtConfig");
            var secretKey = jwtConfig["secret"];
           services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(b =>
                {
                    b.RequireHttpsMetadata = false;
                    b.SaveToken = false;
                    b.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtConfig["validIssuer"],
                        ValidAudience = jwtConfig["validAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    };
                });


            return services;
        }
    }
}
