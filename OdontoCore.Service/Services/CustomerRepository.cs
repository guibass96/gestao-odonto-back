using GBarber.Repository.GenericRepository.Service;
using GBarber.Infrastructure.Data;
using GBarber.Core.Customers.Interfaces;
using GBarber.Core.Customers;

namespace GBarber.Service.Services
{
    public class CustomerRepository : RepositoryBase<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext appContext) : base(appContext)
        {
        }

        public async Task AddCustomer(CustomerEntity customer)
        {

            await CreateAsync(customer);
        }
    }
}
