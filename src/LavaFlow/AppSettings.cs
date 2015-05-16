using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LavaFlow
{
    public static class AppSettings
    {
        public static int ManagementPort
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ManagementPort"]);
            }
        }
    }
}
