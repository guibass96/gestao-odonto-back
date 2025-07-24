using GBarber.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> CreateTenantAsync(string tenantName, AppUser ownerUser);
        Task AssociateUserToTenantAsync(Guid tenantId, AppUser user, string role);
        Task<Tenant?> GetTenantByUserAsync(string userId);
    }
}
