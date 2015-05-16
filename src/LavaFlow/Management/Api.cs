using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace LavaFlow.Management
{
    public class Api : NancyModule
    {
        public Api() : base("admin")
        {
            Get["/status"] = _ => "I am alive!";
        }
    }
}
