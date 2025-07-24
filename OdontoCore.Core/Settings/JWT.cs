using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBarber.Core.Settings
{
    public class JWT
    {
        public string Secret { get; set; }
        public string validIssuer { get; set; }
        public string validAudience { get; set; }
        public double expiresIn { get; set; }

    }
}
