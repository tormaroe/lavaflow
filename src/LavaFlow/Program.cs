using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Ninject;
using Topshelf.Nancy;

namespace LavaFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                // TODO: log4net

                x.UseNinject(new Dependencies());

                x.Service<Daemon>(s =>
                {
                    s.ConstructUsingNinject();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());

                    s.WithNancyEndpoint(x, c =>
                    {
                        c.AddHost(port: AppSettings.ManagementPort);
                        c.ConfigureNancy(nc => nc.UrlReservations.CreateAutomatically = true);
                        c.CreateUrlReservationsOnInstall();
                    });
                });

                x.RunAsNetworkService();

                x.SetDescription("Event sourcing database service");
                x.SetDisplayName("LavaFlow");
                x.SetServiceName("lavaflow");
            });
        }
    }
}
