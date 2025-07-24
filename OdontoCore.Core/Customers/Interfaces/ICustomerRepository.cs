using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Customers.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomer(CustomerEntity customer);
    }
}
