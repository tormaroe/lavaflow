using LavaFlow.Model;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace LavaFlow.Storage
{
    public static class StoragePath
    {
        public static string Get(PersistEvent @event)
        {
            return Path.Combine(GetFolders(@event).ToArray());
        }

        private static IEnumerable<string> GetFolders(PersistEvent @event)
        {
            yield return AppSettings.DataPath;
            yield return @event.AggregateType;
            foreach (var item in @event.AggregateKey.SplitByLength(@event.AggregateKey.Length / 4))
                yield return item;
        }

        private static IEnumerable<string> SplitByLength(this string str, int maxLength)
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
