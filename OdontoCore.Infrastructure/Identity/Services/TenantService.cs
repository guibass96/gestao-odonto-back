using Gbarber.Application.Interfaces;
using GBarber.Core.Entities;
using GBarber.Core.Interfaces;
using GBarber.Core.Settings;
using GBarber.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.SQL.Identity.Services
{
    public class TenantService : ITenantService
    {


        private readonly ITenantRepository _tenantRepository;
        private readonly IRepositoryManager _repo;
        public TenantService(IRepositoryManager repo)
        {
            _repo = repo;
        }

        public Task AssociateUserToTenantAsync(Guid tenantId, AppUser user, string role)
        {
            throw new NotImplementedException();
        }

        public async Task CreateTenant(Tenant tenat)
        {
            await _repo.Tenat.CreateTenat(tenat);
            await _repo.SaveAsync();
        }

        public async Task<Tenant> CreateTenantAsync(string tenantName, AppUser ownerUser)
        {

            try
            {
                var tenantId = Guid.NewGuid();

                var tenant = new Tenant
                {
                    Id = tenantId,
                    Nome = tenantName,
                    DataCriacao = DateTime.UtcNow,
                    TenantUsers = new List<TenantUser>
            {
                new TenantUser
                {
                    TenantId = tenantId,
                    UserId = ownerUser.Id,
                    Role = Roles.TENANT_OWNER.ToString(),

                }
            }
                };
                await _repo.Tenat.CreateTenat(tenant);
                await _repo.SaveAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return new Tenant();
        }

        public Task<Tenant?> GetTenantByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
