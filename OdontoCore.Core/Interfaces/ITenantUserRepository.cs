using GBarber.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Interfaces
{
    public interface ITenantUserRepository : IRepository<TenantUser>
    {
        Task<IEnumerable<TenantUser>> GetUsersByTenantIdAsync(Guid tenantId);
        Task<TenantUser?> GetByUserIdAsync(Guid userId);
        Task<bool> IsUserInTenantAsync(Guid userId, Guid tenantId);
    }
}
