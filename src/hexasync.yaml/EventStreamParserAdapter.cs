using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace hexasync.yaml
{
    public class EventStreamParserAdapter : IParser
    {
        private readonly IEnumerator<ParsingEvent> enumerator;

        public EventStreamParserAdapter(IEnumerable<ParsingEvent> events)
        {
            enumerator = events.GetEnumerator();
        }

        public ParsingEvent Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }
    }
}