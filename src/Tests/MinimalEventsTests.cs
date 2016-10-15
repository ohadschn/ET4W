using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Events;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;

namespace Tests
{
    [TestClass]
    public class MinimalEventsTests
    {
        private static ObservableEventListener m_listener;
        private static MemoryRetentionSink m_sink;
        private static MinimalEvents m_minimialEvents;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            m_minimialEvents = new MinimalEvents();
            m_listener = new ObservableEventListener();
            m_listener.EnableEvents(MinimalEventSource.Log, EventLevel.Verbose);
            m_sink = m_listener.RetainEventRecords().Sink;
        }

        [TestCleanup]
        public void AfterEach()
        {
            m_sink.Clear();
        }

        [ClassCleanup]
        public static void AfterAll()
        {
            m_listener.Dispose();
        }

        [TestMethod]
        public void TestMinimalEvents()
        {
            Assert.AreEqual("OS-Test-Minimal", typeof(MinimalEventSource).GetCustomAttribute<EventSourceAttribute>().Name);
            Util.AssertEventAttributes<MinimalEventSource>("Foo", 1, EventLevel.Informational, null, EventKeywords.None, EventTask.None, null);
            m_minimialEvents.Foo();
            m_sink.AssertEventRecord(1, null, new object[0]);
        }
    }
}
