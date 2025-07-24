using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Interfaces
{
    public interface IUnitOfWork
    {
        ITenantService Contacts { get; }
    }
}
