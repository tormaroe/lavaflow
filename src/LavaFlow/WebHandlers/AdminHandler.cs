using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using LavaFlow.Storage;
using System.IO.Abstractions;

namespace LavaFlow.WebHandlers
{
    public class AdminHandler : NancyModule
    {
        public AdminHandler(StorageActor storage, AppSettingDataPath root) : base("admin")
        {
            Get["/status"] = _ =>
                Response.AsJson(new
                {
                    server = new {
                        version = Program.VERSION.ToString(),
                        started = Program.STARTED,
                        server_time = DateTime.UtcNow,
                        up_time = DateTime.UtcNow - Program.STARTED,
                        stored_count_since_start = storage.StoredCount,
                    },
                    storage = new
                    {
                        queue_length = storage.QueueLength,
                        queue_capasity = AppSettings.StorageQueueLimit,
                        size_in_bytes = root.SizeOnDisk,
                    }
                });
        }
    }
}
