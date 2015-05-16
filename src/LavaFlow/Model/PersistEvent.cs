using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LavaFlow.Model
{
    public class PersistEvent
    {
        public string AggregateType { get; set; }
        public string AggregateKey { get; set; }
        public string EventData { get; set; }
    }
}
