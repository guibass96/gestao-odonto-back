using GBarber.Core.Entities;
using GBarber.Core.Interfaces;
using GBarber.Infrastructure.Data;
using GBarber.Repository.GenericRepository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.SQL.GenericRepository.Service
{
    public class TenantRepository : RepositoryBase<Tenant>, ITenantRepository
    {
        public TenantRepository(AppDbContext appContext) : base(appContext)
        {
        }

        public async Task CreateTenat(Tenant tenant)
        {
            await CreateAsync(tenant);

        }

        public Task<IEnumerable<Tenant>> GetActiveTenantsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tenant?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
