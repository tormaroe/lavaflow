using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LavaFlow.Model;
using Topshelf.Logging;

namespace LavaFlow.Storage
{
    public class StorageActor
    {
        private static readonly LogWriter Logger = HostLogger.Get(typeof(StorageActor));
        private readonly BlockingCollection<PersistEvent> _persistQueue;
        private bool _stopped = false;

        public StorageActor(int capacity)
        {
            _persistQueue = new BlockingCollection<PersistEvent>(boundedCapacity: capacity);

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

        public Func<Stream> GetEventStream(PersistEvent @event)
        {
            var filePath = Path.Combine(StoragePath.Get(@event), @event.Filename);
            Logger.DebugFormat("Preparing stream from {0}", filePath);
            return () => File.OpenRead(filePath);
        }

        public void ProcessEvents()
        {
            Logger.Info("Storage actor starting");
            while (true)
            {
                try
                {
                    var eventToPersist = _persistQueue.Take(); // Blocks if no items to take
                    var path = StoragePath.Get(eventToPersist);
                    var filepath = Path.Combine(path, eventToPersist.Filename);
                    Logger.DebugFormat("Storing <{0}:{1}> length={2} path={3}",
                        eventToPersist.AggregateType,
                        eventToPersist.AggregateKey,
                        eventToPersist.EventData.Length,
                        filepath);
                    try
                    {
                        if (!Directory.Exists(path))
                        {
                            Logger.DebugFormat("Creating new path {0}", path);
                            Directory.CreateDirectory(path);
                        }

                        File.AppendAllText(filepath, eventToPersist.EventData + Environment.NewLine);
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
