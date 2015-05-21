using LavaFlow.Model;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace LavaFlow.Storage
{
    public class DB
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(DB));

        public const string NewLine = "\n";
        
        private readonly IFileSystem io;
        private readonly StoragePath _storagePath;

        public DB(IFileSystem fileSystem, StoragePath storagePath)
        {
            io = fileSystem;
            _storagePath = storagePath;
        }

        public Func<System.IO.Stream> GetEventStream(PersistEvent @event)
        {
            var filePath = io.Path.Combine(_storagePath.Get(@event), @event.Filename);
            Logger.DebugFormat("Preparing stream from {0}", filePath);
            return () => io.File.OpenRead(filePath);
        }

        public IEnumerable<string> GetAllAggregates()
        {
            return io.DirectoryInfo.FromDirectoryName(_storagePath.Root)
                .GetDirectories()
                .Select(di => di.Name);
        }

        public IEnumerable<string> GetKeys(string aggregate)
        {
            try
            {
                return io.DirectoryInfo.FromDirectoryName(io.Path.Combine(_storagePath.Root, aggregate))
                    .GetFiles("*.events", System.IO.SearchOption.AllDirectories)
                    .Select(di => io.Path.GetFileNameWithoutExtension(di.Name));
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Logger.WarnFormat("Aggregate {0} does not exist, returning 0 keys", aggregate);
                return new string[] { };
            }
        }

    }
}
