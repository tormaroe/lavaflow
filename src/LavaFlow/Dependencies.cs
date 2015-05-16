using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LavaFlow
{
    public class Dependencies : NinjectModule
    {
        public override void Load()
        {
            // this.Bind<IWeapon>().To<Sword>();
        }
    }
}
