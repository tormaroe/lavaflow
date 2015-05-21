using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;
using System.IO.Abstractions;

namespace LavaFlow
{
    public class AppSettingDataPath
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(AppSettingDataPath));
        private readonly IFileSystem io;
        private readonly string _value;
        
        public AppSettingDataPath(IFileSystem fileSystem)
        {
            io = fileSystem;

            _value = ConfigurationManager.AppSettings["DataPath"];

            if (!io.Directory.Exists(_value))
            {
                Logger.DebugFormat("Creating data root directory {0}", _value);
                io.Directory.CreateDirectory(_value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public long SizeOnDisk
        {
            get
            {
                var info = io.DirectoryInfo.FromDirectoryName(_value);
                return info
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(fi => fi.Length);
            }
        }
    }

    public static class AppSettings
    {
        public static int Port
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["Port"]);
            }
        }

        public static int StorageQueueLimit
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["StorageQueueLimit"]);
            }
        }
    }
}
