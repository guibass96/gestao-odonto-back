using GBarber.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Entities
{
    public class TenantUser
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string Role { get; set; } 
        public bool IsOwner { get; set; } 
    }
}
