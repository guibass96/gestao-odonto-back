
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GBarber.Core.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string Cpf { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    }
}
