using LavaFlow.Model;
using System.IO.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace LavaFlow.Storage
{
    public class StoragePath
    {
        private readonly IFileSystem io;
        private readonly AppSettingDataPath _root;

        public StoragePath(IFileSystem fileSystem, AppSettingDataPath root)
        {
            io = fileSystem;
            _root = root;
        }

        public string Root
        {
            get
            {
                return _root.Value;
            }
        }

        public string Get(PersistEvent @event)
        {
            return io.Path.Combine(GetFolders(@event).ToArray());
        }

        private IEnumerable<string> GetFolders(PersistEvent @event)
        {
            yield return _root.Value;
            yield return @event.AggregateType;
            foreach (var item in SplitByLength(@event.AggregateKey, @event.AggregateKey.Length / 4))
                yield return item;
        }

        private static IEnumerable<string> SplitByLength(string str, int maxLength)
        {
            if (maxLength < 1)
                maxLength = 1;

            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}
