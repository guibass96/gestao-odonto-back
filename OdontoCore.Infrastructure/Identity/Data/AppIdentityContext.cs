using GBarber.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace GBarber.Infrastructure.Identity.Data
{
    public class AppIdentityContext : IdentityDbContext<AppUser>
    {

        public AppIdentityContext(DbContextOptions<AppIdentityContext> options) : base(options)
        {
        }


    }
}
