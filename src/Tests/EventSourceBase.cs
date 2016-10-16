using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System.Diagnostics.Tracing;

namespace Tests
{
    public abstract class EventSourceBase : EventSource
    {
        private ObservableEventListener m_listener;
        public MemoryRetentionSink Sink { get; private set; }

        public void DisposeListener()
        {
            m_listener.Dispose();
        }

        protected EventSourceBase()
        {
            m_listener = new ObservableEventListener();
            m_listener.EnableEvents(this, EventLevel.Verbose);
            Sink = m_listener.RetainEventRecords().Sink;
        }
    }
}
