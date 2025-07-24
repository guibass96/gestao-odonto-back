using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Entities
{
    public class Tenant
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        [MaxLength(255)]
        public string Nome { get; set; } = null!;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();

    }
}
