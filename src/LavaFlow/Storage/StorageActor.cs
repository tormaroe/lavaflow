using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LavaFlow.Model;
using Topshelf.Logging;
using System.IO.Abstractions;

namespace LavaFlow.Storage
{
    public class StorageActor
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(StorageActor));
        private readonly BlockingCollection<PersistEvent> _persistQueue;
        private readonly IFileSystem io;
        private readonly StoragePath _storagePath;
        private bool _stopped = false;

        public StorageActor(int capacity, IFileSystem fileSystem, StoragePath storagePath)
        {
            io = fileSystem;
            _storagePath = storagePath;
            _persistQueue = new BlockingCollection<PersistEvent>(boundedCapacity: capacity);

            Logger.Info("Storage actor starting");
            Task.Factory.StartNew(ProcessEvents);
        }

        public void Add(PersistEvent item)
        {
            if (_stopped)
                throw new InvalidOperationException("Storage agent is shutting down, can't accept any more events.");

            _persistQueue.Add(item);
        }

        public void PrepareForShutdown()
        {
            _stopped = true;
        }

        public int QueueLength
        {
            get
            {
                return _persistQueue.Count;
            }
        }

        public void ProcessEvents()
        {
            Logger.Info("Storage actor started");
            while (true)
            {
                try
                {
                    var eventToPersist = _persistQueue.Take(); // Blocks if no items to take
                    var path = _storagePath.Get(eventToPersist);
                    var filepath = io.Path.Combine(path, eventToPersist.Filename);
                    Logger.DebugFormat("Storing event to <{0}:{1}> length={2}",
                        eventToPersist.AggregateType,
                        eventToPersist.AggregateKey,
                        eventToPersist.EventData.Length,
                        filepath);
                    try
                    {
                        if (!io.Directory.Exists(path))
                        {
                            Logger.DebugFormat("Creating new path {0}", path);
                            io.Directory.CreateDirectory(path);
                        }

                        io.File.AppendAllText(filepath, eventToPersist.EventData + DB.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("Exception in persistance loop, event <{0}:{1}> lost", 
                            eventToPersist.AggregateType, 
                            eventToPersist.AggregateKey), ex);
                        Logger.ErrorFormat("Event data: {0}", eventToPersist.EventData);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception in persistance loop", ex);
                }
            }
        }
    }
}
