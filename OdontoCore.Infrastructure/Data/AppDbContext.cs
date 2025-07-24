using GBarber.Core.Customers;
using GBarber.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
namespace GBarber.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenant");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Id)
                    .HasDefaultValueSql("gen_random_uuid()"); 

                entity.Property(t => t.Nome)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(t => t.DataCriacao)
                    .HasDefaultValueSql("now()"); 
            });

            modelBuilder.Entity<TenantUser>(builder =>
            {
                builder.HasKey(tu => new { tu.TenantId, tu.UserId }); 

                builder
                    .HasOne(tu => tu.Tenant)
                    .WithMany(t => t.TenantUsers)
                    .HasForeignKey(tu => tu.TenantId);

                builder
                    .HasOne(tu => tu.User)
                    .WithMany(u => u.TenantUsers)
                    .HasForeignKey(tu => tu.UserId);

                builder.Property(tu => tu.Role).HasMaxLength(100); // opcional
                builder.Property(tu => tu.IsOwner).HasDefaultValue(false); // opcional
            });
        }

        public DbSet<CustomerEntity> Customer { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantUser> TenantUser { get; set; }
    }


  
}
