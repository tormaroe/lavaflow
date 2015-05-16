﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Nancy;
using Topshelf.Logging;
using LavaFlow.Storage;
using LavaFlow.Model;

namespace LavaFlow.WebHandlers
{
    public class EventApiHandler : NancyModule
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(EventApiHandler));
        private readonly StorageActor _storage;

        public EventApiHandler(StorageActor storage) : base("events")
        {
            _storage = storage;

            base.Before.AddItemToEndOfPipeline(context =>
            {
                Logger.InfoFormat("{0} {1}",
                    context.Request.Method,
                    context.Request.Path);
                return null;
            });

            base.After.AddItemToEndOfPipeline(context =>
            {
                Logger.DebugFormat(" ==> {0}", 
                    context.Response.StatusCode);
            });

            Post["/{aggregate}/{key}"] = p =>
            {
                storage.Add(new PersistEvent
                {
                    AggregateType = p.aggregate,
                    AggregateKey = p.key,
                    EventData = GetEventData(Request.Body)
                });
                return 201;
            };

            Get["/{aggregate}/{key}"] = p =>
                Response.FromStream(storage.GetEventStream(new PersistEvent
                {
                    AggregateType = p.aggregate,
                    AggregateKey = p.key,
                }), "text/plain");

            Get["/"] = _ =>
                Response.AsJson(
                    new DirectoryInfo(AppSettings.DataPath)
                        .GetDirectories()
                        .Select(di => di.Name));

            Get["/{aggregate}"] = p =>
                Response.AsJson(
                    new DirectoryInfo(Path.Combine(AppSettings.DataPath, p.aggregate))
                        .GetFiles("*.events", SearchOption.AllDirectories)
                        .Select(di => Path.GetFileNameWithoutExtension(di.Name)));
        }

        private string GetEventData(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return reader
                    .ReadToEnd()
                    .Replace("\n", " ");
        }
    }
}
