using LavaFlow.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace LavaFlow.Storage
{
    public static class DB
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(DB));

        public const string NewLine = "\n";

        public static Func<Stream> GetEventStream(PersistEvent @event)
        {
            var filePath = Path.Combine(StoragePath.Get(@event), @event.Filename);
            Logger.DebugFormat("Preparing stream from {0}", filePath);
            return () => File.OpenRead(filePath);
        }

        public static IEnumerable<string> GetAllAggregates()
        {
            return new DirectoryInfo(AppSettings.DataPath)
                .GetDirectories()
                .Select(di => di.Name);
        }

        public static IEnumerable<string> GetKeys(string aggregate)
        {
            try
            {
                return new DirectoryInfo(Path.Combine(AppSettings.DataPath, aggregate))
                    .GetFiles("*.events", SearchOption.AllDirectories)
                    .Select(di => Path.GetFileNameWithoutExtension(di.Name));
            }
            catch (DirectoryNotFoundException)
            {
                Logger.WarnFormat("Aggregate {0} does not exist, returning 0 keys", aggregate);
                return new string[] { };
            }
        }

    }
}
