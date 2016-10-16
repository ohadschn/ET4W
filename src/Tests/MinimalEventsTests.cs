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
        private static ObservableEventListener s_listener;
        private static MemoryRetentionSink s_sink;
        private static MinimalEvents s_minimialEvents;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            s_minimialEvents = new MinimalEvents();
            s_listener = new ObservableEventListener();
            s_listener.EnableEvents(MinimalEventSource.Log, EventLevel.Verbose);
            s_sink = s_listener.RetainEventRecords().Sink;
        }

        [TestCleanup]
        public void AfterEach()
        {
            s_sink.Clear();
        }

        [ClassCleanup]
        public static void AfterAll()
        {
            s_listener.Dispose();
        }

        [TestMethod]
        public void TestMinimalEvents()
        {
            Assert.AreEqual("OS-Test-Minimal", typeof(MinimalEventSource).GetCustomAttribute<EventSourceAttribute>().Name, "Mismatched event source name");
            Util.AssertEventAttributes<MinimalEventSource>("Foo", 1, EventLevel.Informational, null, EventKeywords.None, EventTask.None, null);
            s_minimialEvents.Foo();
            s_sink.AssertEventRecord(1, null, new object[0]);
        }
    }
}
