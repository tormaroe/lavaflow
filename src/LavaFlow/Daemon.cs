using LavaFlow.Storage;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace LavaFlow
{
    public class Daemon
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(Daemon));

        private readonly StorageActor _storage;
        private readonly StoragePath _storagePath;

        public Daemon(StorageActor storage, StoragePath storagePath)
        {
            _storage = storage;
            _storagePath = storagePath;
        }

        private void LogHeader()
        {
            Logger.InfoFormat(@"

                   xl""``""lx
                  X8X::::xxX8X
                  8X8::X8bd8X8               LAVAFLOW SERVER v{0}
                 dX8::::XbdX8Xb              
                dX8::d8::XbdX8Xb             Copyright (c) 2015 Torbjørn Marø
               dX8::d8X8::XbdX8Xb            Licensed under The MIT License
             .dX:::d8X8X8:::bdX8Xb.          
           .d8:::b:::8X8X::::bdX8X8b.        Admin URL: http://localhost:{1}/admin
       _.-dX8:::bd8::X8X::8X::dbX8X8Xb-._
    .-d8X8X::8bdX:::X8X::8X8X::8db8X8X8X8b-.
 .-d8X8X8::::bdX8X:::8X8::X8X8::X8db8X8X8-RG-b-.

         Port              :  {1}
         DataPath          :  {2}
         StorageQueueLimit :  {3}
",
                Program.VERSION,
                AppSettings.Port,
                _storagePath.Root,
                AppSettings.StorageQueueLimit);
        }

        public void Start()
        {
            LogHeader();
            Logger.Info("Lava now flowing!");
        }

        public void Stop()
        {
            int waitedSoFar = 0;
            const int waitInterval = 1000; 
            const int maxWait = 30000;
            while (waitedSoFar < maxWait)
            {
                int queueLength = _storage.QueueLength;
                if (_storage.QueueLength == 0)
                {
                    Logger.Info("Storage queue empty");
                    break;
                }

                Logger.WarnFormat(
                    "{0} events in storage queue, aborting in {2}",
                    (maxWait - waitedSoFar) / 1000);

                Thread.Sleep(waitInterval);
                waitedSoFar += waitInterval;
            }

            Logger.Info("Lava flow stopped!");
        }
    }
}
