using GBarber.Core.Entities;
using GBarber.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application.Interfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
    }
}
