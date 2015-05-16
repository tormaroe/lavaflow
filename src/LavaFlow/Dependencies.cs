using Nancy;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LavaFlow.Storage;

namespace LavaFlow
{
    public class Dependencies : NinjectModule
    {
        public override void Load()
        {
            // this.Bind<IWeapon>().To<Sword>();
        }
    }

    public class NancyDependencies : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<StorageActor>(new StorageActor(capacity: AppSettings.StorageQueueLimit));
        }
    }
}
