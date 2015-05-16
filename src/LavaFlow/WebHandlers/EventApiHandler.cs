﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Nancy;
using Topshelf.Logging;
using LavaFlow.Storage;

namespace LavaFlow.WebHandlers
{
    public class EventApiHandler : NancyModule
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(EventApiHandler));
        private readonly StorageActor _storage;

        public EventApiHandler(StorageActor storage)
        {
            _storage = storage;

            Post["/{aggregate}/{key}/event"] = p =>
            {
                Logger.InfoFormat("POST {0}", Request.Path);
                storage.Add(new Model.PersistEvent
                {
                    AggregateType = p.aggregate,
                    AggregateKey = p.key,
                    EventData = GetEventData(Request.Body)
                });
                return 201;
            };
        }

        private string GetEventData(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return reader
                    .ReadToEnd()
                    .Replace(Environment.NewLine, " ");
        }
    }
}