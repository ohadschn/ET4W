using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Tests.Events;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using System;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System.Globalization;

namespace Tests
{
    public class Session
    {
        public Guid Id { get; set; }
    }
    public class CustomType
    {
        public string Foo { get; set; }
    }

    public class AnotherCustomType
    {
        public int Bar { get; set; }
    }

    public enum TestEnum
    {
        Hello = 0,
        World = 1
    }

    [TestClass]
    public class EventTests
    {
        static Guid m_sessionId;
        static MemoryRetentionSink m_sink;
        static ObservableEventListener m_listener;
        static TestsEvents m_testsEvents;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            m_sessionId = Guid.NewGuid();
            m_testsEvents = new TestsEvents(ct => ct.Foo, act => act.Bar, te => (int)te, s => s.Id, () => new Session { Id = m_sessionId }, () => new Session());
            m_listener = new ObservableEventListener();
            m_listener.EnableEvents(TestsEventSource.Log, EventLevel.Verbose);
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
            const string expectedMessage = "Event with no keywords";
            Util.AssertEventAttributes<TestsEventSource>("FooInfo", 1, EventLevel.Informational, expectedMessage, EventKeywords.None, TestsEventSource.Tasks.Foo, EventOpcode.Info);
            var context = Guid.NewGuid();
            m_testsEvents.FooInfo(context);
            m_sink.AssertEventRecord(1, expectedMessage, new object[] { context, m_sessionId, Guid.Empty });
        }

        [TestMethod]
        public void TestNoTask()
        {
            const string expectedMessage = "Event with no task";
            Util.AssertEventAttributes<TestsEventSource>("NoTask", 2, EventLevel.Warning, expectedMessage, TestsEventSource.Keywords.Raz, EventTask.None, EventOpcode.Info);
            var context = Guid.NewGuid();
            m_testsEvents.NoTask(context);
            m_sink.AssertEventRecord(2, expectedMessage, new object[] { context, m_sessionId, Guid.Empty });
        }

        [TestMethod]
        public void TestNoOpcode()
        {
            const string expectedMessage = "Event with no Opcode";
            Util.AssertEventAttributes<TestsEventSource>("NoOpcode", 3, EventLevel.Error, expectedMessage, TestsEventSource.Keywords.Baz | TestsEventSource.Keywords.Faz, TestsEventSource.Tasks.Boo, null);
            var context = Guid.NewGuid();
            m_testsEvents.NoOpcode(context);
            m_sink.AssertEventRecord(3, expectedMessage, new object[] { context, m_sessionId, Guid.Empty });
        }

        [TestMethod]
        public void TestNativeParameters()
        {
            const string message = "context: {0}, session: {1}, session2: {2}, b: {3}, c: {4}, sb: {5}, by: {6}, i16: {7}, ui16: {8}, i32: {9}, ui32: {10}, i64: {11}, ui64: {12}, sin: {13}, d: {14}, s: {15}, g: {16}, ptr: {17}";
            Util.AssertEventAttributes<TestsEventSource>("Parameters", 4, EventLevel.Informational, message, EventKeywords.None, EventTask.None, null);
            //DateTime and byte[] are not tested due to EventAnalyzer issues that should be fixed in the next release:
            //https://github.com/mspnp/semantic-logging/pull/83
            //https://github.com/mspnp/semantic-logging/pull/94
            var context = Guid.NewGuid();
            var guid = Guid.NewGuid();
            m_testsEvents.Parameters(context, true, 'a', 1, 2, 3, 4, 5, 6, 7, 8, 1.0f, 2.3, "foo", guid, IntPtr.Zero);
            m_sink.AssertEventRecord(4, String.Format(CultureInfo.InvariantCulture, message, context, m_sessionId, Guid.Empty, true, 'a', 1, 2, 3, 4, 5, 6, 7, 8, 1.0f, 2.3, "foo", guid, IntPtr.Zero), 
                new object[] {
                    context, m_sessionId, Guid.Empty, true, 'a', (sbyte)1, (byte)2, (short)3, (ushort)4, (int)5, (uint)6, (long)7, (ulong)8, 1.0f, 2.3, "foo", guid, IntPtr.Zero
                });
        }

        [TestMethod]
        public void TestCustomTypes()
        {
            Util.AssertEventAttributes<TestsEventSource>("CustomTypes", 5, EventLevel.Critical, null, EventKeywords.None, EventTask.None, null);
            var context = Guid.NewGuid();
            m_testsEvents.CustomTypes(context, new CustomType { Foo = "foo" }, 1.0, new AnotherCustomType() { Bar = 42 }, TestEnum.World);
            m_sink.AssertEventRecord(5, null, new object[] { context, m_sessionId, Guid.Empty, "foo", 1.0, 42, 1 });
        }
    }
}