using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class Config
    {
        public static string MesosIp
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["mesos.ip"]);
            }
        }
    }
}
