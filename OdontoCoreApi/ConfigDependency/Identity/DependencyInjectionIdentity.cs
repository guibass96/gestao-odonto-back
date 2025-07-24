using GBarber.Core.Entities;
using GBarber.Infrastructure.Data;
using GBarber.Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;

namespace GBarber.WebApi.ConfigDependency.Identity
{
    public static class DependencyInjectionIdentity
    {
        public static IServiceCollection AddApplicationServicesIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Configuração de senha
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Confirmação de e-mail obrigatória
                options.SignIn.RequireConfirmedEmail = true;

                // Lockout (bloqueio após tentativas erradas)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configuração de usuário
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = true; // se quiser exigir email único, coloque como true
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders(); // ✅ Usa DataProtectorTokenProvider para e-mail, senha etc.

            // Tempo de vida dos tokens (confirmação de e-mail, reset de senha, etc.)
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12); 
            });

      

            return services;
        }
    }
}
