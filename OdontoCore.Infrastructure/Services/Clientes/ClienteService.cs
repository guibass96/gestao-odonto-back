using Gbarber.Application.Interfaces.Clientes;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GBarber.Infrastructure.Data;
using GBarber.Core.Customers;
namespace GBarber.Infrastructure.Services.Clientes
{
    public class ClienteService
    {
        protected AppDbContext RepositoryContext;
        public async Task AddClient(CustomerEntity cliente)
        {
            RepositoryContext.Customer.Add(cliente);
        }

        public Task EditClient(CustomerEntity cliente)
        {
            throw new NotImplementedException();
        }

        public Task GetAllClients()
        {
            throw new NotImplementedException();
        }

        public Task GetById(CustomerEntity cliente)
        {
            throw new NotImplementedException();
        }

    }
}
