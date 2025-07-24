using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gbarber.Application
{
    public class ApplicationAssemblyReference
    {
        public static readonly Assembly Assembly = typeof(ApplicationAssemblyReference).Assembly;
    }
}
