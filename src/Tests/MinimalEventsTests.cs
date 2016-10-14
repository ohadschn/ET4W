using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Events;
using System.Diagnostics.Tracing;

namespace Tests
{
    [TestClass]
    public class MinimalEventsTests
    {
        MinimalEvents m_minimialEvents;

        [TestInitialize]
        public void BeforeEach()
        {
            m_minimialEvents = new MinimalEvents();
        }

        [TestMethod]
        public void TestMinimalEvents()
        {
            Assert.AreEqual("OS-Test-Minimal", typeof(MinimalEventSource).GetCustomAttribute<EventSourceAttribute>().Name);
            Util.AssertEventAttributes<MinimalEventSource>("Foo", 1, EventLevel.Informational, null, EventKeywords.None, EventTask.None, null);
            m_minimialEvents.Foo();
        }
    }
}
