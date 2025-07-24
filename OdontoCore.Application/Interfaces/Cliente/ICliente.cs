using GBarber.Core.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gbarber.Application.Interfaces.Clientes
{
    public interface ICliente
    {
        Task AddClient(CustomerEntity cliente);
        Task EditClient(CustomerEntity cliente);
        Task GetAllClients();
        Task GetById(CustomerEntity cliente);
    }
}
