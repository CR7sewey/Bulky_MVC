using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public class StripeSettings
    {
        public string PublishableKey { get; set; } // injection dependency from appsettings.json file (in program.cs)
        public string SecretKey { get; set; }
    }
}
