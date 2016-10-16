using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Tests
{
    public sealed class MemoryRetentionSink : IObserver<EventEntry>
    {
        public ConcurrentBag<EventRecord> RetainedEvents { get; private set; }
        public ConcurrentBag<Exception> RetainedErrors { get; private set; }

        public MemoryRetentionSink()
        {
            Clear();
        }

        public void Clear()
        {
            RetainedEvents = new ConcurrentBag<EventRecord>();
            RetainedErrors = new ConcurrentBag<Exception>();
        }

        public void OnNext(EventEntry value)
        {
            if (value == null)
            {
                RetainedErrors.Add(new InvalidOperationException("null value received in OnNext"));
                return;
            }

            RetainedEvents.Add(
                new EventRecord(value.EventId, value.FormattedMessage, new ReadOnlyCollection<object>(value.Payload.ToArray()), value.Timestamp));
        }

        public void OnError(Exception error)
        {
            RetainedErrors.Add(error);
        }

        public void OnCompleted()
        {
            //nothing to do
        }
    }

    public static class SessionErrorSinkExtensions
    {
        public static SinkSubscription<MemoryRetentionSink> RetainEventRecords(this IObservable<EventEntry> eventStream)
        {
            if (eventStream == null) throw new ArgumentNullException(nameof(eventStream));

            var sink = new MemoryRetentionSink();
            return new SinkSubscription<MemoryRetentionSink>(eventStream.Subscribe(sink), sink);
        }
    }
}
