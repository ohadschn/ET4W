using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Tests.EventSources;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using System;

namespace Tests
{
    [TestClass]
    public class EventTests
    {
        private void AssertEventAttributes(string eventName, int expectedId, EventLevel expectedLevel, string expectedMessage, 
            EventKeywords expectedKeywords, EventTask expectedTask, EventOpcode? expectedOpcode)
        {
            var eventAttribute = typeof(TestsEventSource).GetMethod(eventName).GetCustomAttribute<EventAttribute>();
            Assert.AreEqual(expectedId, eventAttribute.EventId, "Mismatched event ID");
            Assert.AreEqual(expectedLevel, eventAttribute.Level, "Mismatched event level");
            Assert.AreEqual(expectedMessage, eventAttribute.Message, "Mismatched event message");
            Assert.AreEqual(expectedKeywords, eventAttribute.Keywords, "Mismatched event keywords");
            Assert.AreEqual(expectedTask, eventAttribute.Task, "Mismatched event task");

            if (expectedOpcode == null)
            {
                bool opcodeSet = (bool)typeof(EventAttribute).GetProperty("IsOpcodeSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(eventAttribute);
                Assert.IsFalse(opcodeSet, "Opcode was set even though it was not specified in the event definition");
            }
            else
            {
                Assert.AreEqual(expectedOpcode, eventAttribute.Opcode, "Mismatched event opcode");
            }
        }

        [TestMethod]
        public void AnalyzeEventSource()
        {
            EventSourceAnalyzer.InspectAll(TestsEventSource.Log);
        }

        [TestMethod]
        public void TestSourceName()
        {
            Assert.AreEqual("OS-Test-Foo", typeof(TestsEventSource).GetCustomAttribute<EventSourceAttribute>().Name);
        }

        [TestMethod]
        public void TestNoKeywords()
        {
            AssertEventAttributes("NoKeyWords", 1, EventLevel.Informational, "Event with no keywords", EventKeywords.None, TestsEventSource.Tasks.Foo, EventOpcode.Info);
            new TestsEvents().NoKeyWords();
        }

        [TestMethod]
        public void TestNoTask()
        {
            AssertEventAttributes("NoTask", 2, EventLevel.Warning, "Event with no task", TestsEventSource.Keywords.Raz, EventTask.None, EventOpcode.Info);
            new TestsEvents().NoOpcode();
        }

        [TestMethod]
        public void TestNoOpcode()
        {
            AssertEventAttributes("NoOpcode", 3, EventLevel.Error, "Event with no Opcode", TestsEventSource.Keywords.Baz | TestsEventSource.Keywords.Faz, TestsEventSource.Tasks.Boo, null);
            new TestsEvents().NoTask();
        }

        [TestMethod]
        public void TestNativeParameters()
        {
            new Guid();
            AssertEventAttributes("Parameters", 4, EventLevel.Informational, null, EventKeywords.None, EventTask.None, null);
            //DateTime and byte[] are not tested due to an EventAnalyzer issues that should be fixed in the next release
            new TestsEvents().Parameters(true, 'a', 1, 2, 3, 4, 5, 6, 7, 8, 1.0f, 2.3, "foo", Guid.NewGuid(), IntPtr.Zero);
        }
    }
}
