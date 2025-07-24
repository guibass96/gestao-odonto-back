using GBarber.Core;
using GBarber.Core.Customers.Interfaces;
using GBarber.Core.Interfaces;
using GBarber.Infrastructure.Data;
using GBarber.SQL.GenericRepository.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Service.Services
{
    public class RepositoryManager : IRepositoryManager
    {
        private AppDbContext _repositoryContext;
        private ICustomerRepository _customerRepository;
        private ITenantRepository _tenantRepository;

        public RepositoryManager(AppDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
          

        }
        public ICustomerRepository Customer
        {
            get
            {
                if (_customerRepository is null)
                    _customerRepository = new CustomerRepository(_repositoryContext);
                return _customerRepository;
            }
        }  
        
        public ITenantRepository Tenat
        {
            get
            {
                if (_tenantRepository is null)
                    _tenantRepository = new TenantRepository(_repositoryContext);
                return _tenantRepository;
            }
        }


        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
    }
}
