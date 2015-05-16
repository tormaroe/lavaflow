using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace LavaFlow
{
    public static class AppSettings
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(AppSettings));
        private static string _dataPath;
        private static object _padlock = new object();

        public static string DataPath
        {
            get
            {
                if (_dataPath != null)
                    return _dataPath;

                lock (_padlock)
                {
                    _dataPath = ConfigurationManager.AppSettings["DataPath"];

                    if (!Directory.Exists(_dataPath))
                    {
                        Logger.DebugFormat("Creating data root directory {0}", _dataPath);
                        Directory.CreateDirectory(_dataPath);
                    }

                    return _dataPath;
                }
            }
        }

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
