using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Framework_Info
    {
        public string user { get; set; }
        public string name { get; set; }
        public string[] roles { get; set; }
        public Capability[] capabilities { get; set; }
    }
}
