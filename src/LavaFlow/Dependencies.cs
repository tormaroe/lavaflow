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
    public static class BaseDependencies
    {
        public static IFileSystem fileSystem = new FileSystem();

        public static StorageActor storageActor = new StorageActor(
            capacity: AppSettings.StorageQueueLimit,
            fileSystem: fileSystem,
            storagePath: new StoragePath(fileSystem, new AppSettingDataPath(fileSystem)));
    }

    public class ServiceDependencies : NinjectModule
    {
        public override void Load()
        {
            this.Bind<StorageActor>().ToConstant(BaseDependencies.storageActor);
            this.Bind<IFileSystem>().ToConstant(BaseDependencies.fileSystem);
        }
    }

    public class NancyDependencies : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<StorageActor>(BaseDependencies.storageActor);
            container.Register<IFileSystem>(BaseDependencies.fileSystem);
        }
    }
}
