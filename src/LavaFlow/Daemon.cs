using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace LavaFlow
{
    public class Daemon
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(Daemon));

        public void Start()
        {
            Logger.InfoFormat(@"

                   xl""``""lx
                  X8X::::xxX8X
                  8X8::X8bd8X8               LAVAFLOW SERVER v{0}
                 dX8::::XbdX8Xb              Copyright (c) 2015 Torbjørn Marø
                dX8::d8::XbdX8Xb             Licensed under The MIT License
               dX8::d8X8::XbdX8Xb
             .dX:::d8X8X8:::bdX8Xb.          Admin URL: http://localhost:{1}/admin
           .d8:::b:::8X8X::::bdX8X8b.
       _.-dX8:::bd8::X8X::8X::dbX8X8Xb-._
    .-d8X8X::8bdX:::X8X::8X8X::8db8X8X8X8b-.
 .-d8X8X8::::bdX8X:::8X8::X8X8::X8db8X8X8-RG-b-.

", 
                Assembly.GetExecutingAssembly().GetName().Version,
                AppSettings.ManagementPort);

            Logger.Info("Lava now flowing!");
        }

        public void Stop()
        {
            Logger.Info("Lava flow stopped!");
        }
    }
}
