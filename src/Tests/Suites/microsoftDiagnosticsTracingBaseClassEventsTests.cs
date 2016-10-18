using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Diagnostics.Tracing;
using MicrosoftDiagnosticsTracingTests.Events;

namespace Tests
{
    public abstract class MicrosoftDiagnosticsTracingEventSourceBase : EventSource
    {
    }

    [TestClass]
    public class MicrosoftDiagnosticsTracingBaseClassEventsTests
    {
        [TestMethod]
        public void TestMicrosoftDiagnosticsTracingBaseClassEvents()
        {
            //minimal tests to check compilation and basic behavior - see the comment in the MicrosoftDiagnosticsTracingEventsTests class for rationale

            Assert.IsInstanceOfType(MinimalEventSource.Log, typeof(MicrosoftDiagnosticsTracingEventSourceBase), "Event source class doesn't inherit from the expected base class");
            new MinimalEvents().Foo();
        }
    }
}
