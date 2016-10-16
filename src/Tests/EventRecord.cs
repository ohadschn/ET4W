using System;
using System.Collections.ObjectModel;

namespace Tests
{
    public class EventRecord
    {
        public int Id { get; private set; }
        public string FormattedMessage { get; private set; }
        public ReadOnlyCollection<object> Payload { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }

        public EventRecord(int id, string formattedMessage, ReadOnlyCollection<object> payload, DateTimeOffset timestamp)
        {
            Id = id;
            FormattedMessage = formattedMessage;
            Payload = payload;
            Timestamp = timestamp;
        }
    }
}
