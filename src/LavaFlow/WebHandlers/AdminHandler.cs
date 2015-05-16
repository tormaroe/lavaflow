using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace LavaFlow.WebHandlers
{
    public class AdminHandler : NancyModule
    {
        public AdminHandler() : base("admin")
        {
            Get["/status"] = _ => "I am alive!";
        }
    }
}
