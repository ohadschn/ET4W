using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftDiagnosticsTracingTests.Events;
using System;

namespace Tests
{
    [TestClass]
    public class MicrosoftDiagnosticsTracingEventsTests
    {
        [TestMethod]
        public void TestMicrosoftDiagnosticsTracingEvents()
        {
            //there's no point in validating the attributes again, it's the exact same code we already tested with System.Diagnostics.Tracing (only the namespace changed)
            //we also can't use SLAB to test the calls as it is compiled against System.Diagnostics.Tracing, but again it's basically the same code anyway
            //the main thing we want to make sure is that the code compiles against Microsoft.Diagnostics.Tracing and that we can make the event calls we expect

            var guid = Guid.NewGuid();
            var testsEvents = new TestsEvents(ct => ct.Foo, act => act.Bar, te => (int)te, s => s.Id, () => new Session { Id = guid }, () => new Session());
            testsEvents.FooInfo(guid);
            testsEvents.NoTask(guid);
            testsEvents.NoOpcode(guid);
            testsEvents.Parameters(guid, true, 'a', 1, 2, 3, 4, 5, 6, 7, 8, 1.0f, 2.3, "foo", guid, IntPtr.Zero);
            testsEvents.CustomTypes(guid, new CustomType { Foo = "foo" }, 1.0, new AnotherCustomType() { Bar = 42 }, Greeting.Hello);
            testsEvents.Channel(guid);
        }
    }
}
