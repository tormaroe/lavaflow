using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Ninject;
using Topshelf.Nancy;
using Topshelf.Logging;
using System.Reflection;

namespace LavaFlow
{
    class Program
    {
        public static Version VERSION = Assembly.GetExecutingAssembly().GetName().Version;
        public static DateTime STARTED;

        static void Main(string[] args)
        {
            STARTED = DateTime.UtcNow;

            HostFactory.Run(x =>
            {
                x.UseLog4Net("log4net.config", watchFile: true);
                x.UseNinject(new ServiceDependencies());

                x.Service<Daemon>(s =>
                {
                    s.ConstructUsingNinject();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());

                    s.WithNancyEndpoint(x, c =>
                    {
                        c.AddHost(port: AppSettings.Port);
                        c.ConfigureNancy(nc => nc.UrlReservations.CreateAutomatically = true);
                        c.CreateUrlReservationsOnInstall();
                    });
                });

                x.RunAsNetworkService();
                x.StartAutomatically();

                x.SetDescription("Event sourcing database service");
                x.SetDisplayName("LavaFlow");
                x.SetServiceName("lavaflow");

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(delayInMinutes: 0);
                    r.RestartService(delayInMinutes: 0);
                    r.RestartService(delayInMinutes: 1);

                    r.SetResetPeriod(days: 1);
                });
            });
        }
    }
}
