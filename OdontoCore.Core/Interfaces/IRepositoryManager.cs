using GBarber.Core.Customers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Interfaces
{
    public interface IRepositoryManager
    {
        ICustomerRepository Customer { get; }
        ITenantRepository Tenat { get; }
        Task SaveAsync();
    }
}
