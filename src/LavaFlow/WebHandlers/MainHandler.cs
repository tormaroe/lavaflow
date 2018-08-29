using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LavaFlow.WebHandlers
{
    public class MainHandler : NancyModule
    {
        public MainHandler()
        {
            Get["/"] = _ => View["main"];
        }
    }
}
