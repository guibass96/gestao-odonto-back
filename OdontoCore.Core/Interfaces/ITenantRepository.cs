using GBarber.Core.Entities;

namespace GBarber.Core.Interfaces
{
    public interface ITenantRepository 
    {
        Task<Tenant?> GetByNameAsync(string name);
        Task<IEnumerable<Tenant>> GetActiveTenantsAsync();
        Task CreateTenat(Tenant tenant);
    }
}
