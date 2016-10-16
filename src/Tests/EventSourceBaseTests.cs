using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Events;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class EventSourceBaseTests
    {
        private static BaseClassEvents s_baseClassEvents;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            s_baseClassEvents = new BaseClassEvents();
        }

        [TestCleanup]
        public void AfterEach()
        {
            BaseClassEventSource.Log.Sink.Clear();
        }

        [ClassCleanup]
        public static void AfterAll()
        {
            BaseClassEventSource.Log.DisposeListener();
        }

        [TestMethod]
        public void TestBaseEventSource()
        {
            Assert.IsInstanceOfType(BaseClassEventSource.Log, typeof(EventSourceBase), "Event source class doesn't inherit from the expected base class");

            Assert.AreEqual("OS-Test-BaseClass", typeof(BaseClassEventSource).GetCustomAttribute<EventSourceAttribute>().Name, "Mismatched event source name");
            Util.AssertEventAttributes<BaseClassEventSource>("Foo", 10, EventLevel.Informational, "Foo: {0}", EventKeywords.None, EventTask.None, null);
            s_baseClassEvents.Foo("bar");
            BaseClassEventSource.Log.Sink.AssertEventRecord(10, "Foo: bar", new object[] { "bar" });
        }
    }
}
