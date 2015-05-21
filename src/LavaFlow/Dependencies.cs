using Nancy;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LavaFlow.Storage;
using System.IO.Abstractions;

namespace LavaFlow
{
    public class Dependencies : NinjectModule
    {
        public static StorageActor storageActor = new StorageActor(
            capacity: AppSettings.StorageQueueLimit,
            fileSystem: new FileSystem(),
            storagePath: new StoragePath(new FileSystem(), new AppSettingDataPath(new FileSystem())));

        public override void Load()
        {
            this.Bind<StorageActor>().ToConstant(storageActor);
            this.Bind<IFileSystem>().ToConstant(new FileSystem());
        }
    }

    public class NancyDependencies : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<StorageActor>(Dependencies.storageActor);
            container.Register<IFileSystem>(new FileSystem());
        }
    }
}
