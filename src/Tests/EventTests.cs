﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Tests.Events;
using System.Diagnostics.Tracing;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using System;

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
        TestsEvents m_testsEvents;

        [TestInitialize]
        public void BeforeEach()
        {
            m_testsEvents = new TestsEvents(ct => ct.Foo, act => act.Bar, te => (int)te, s => s.Id, () => new Session { Id = Guid.NewGuid() }, () => new Session() );
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
            Util.AssertEventAttributes<TestsEventSource>("NoKeyWords", 1, EventLevel.Informational, "Event with no keywords", EventKeywords.None, TestsEventSource.Tasks.Foo, EventOpcode.Info);
            m_testsEvents.NoKeyWords(Guid.Empty);
        }

        [TestMethod]
        public void TestNoTask()
        {
            Util.AssertEventAttributes<TestsEventSource>("NoTask", 2, EventLevel.Warning, "Event with no task", TestsEventSource.Keywords.Raz, EventTask.None, EventOpcode.Info);
            m_testsEvents.NoOpcode(Guid.Empty);
        }

        [TestMethod]
        public void TestNoOpcode()
        {
            Util.AssertEventAttributes<TestsEventSource>("NoOpcode", 3, EventLevel.Error, "Event with no Opcode", TestsEventSource.Keywords.Baz | TestsEventSource.Keywords.Faz, TestsEventSource.Tasks.Boo, null);
            m_testsEvents.NoTask(Guid.Empty);
        }

        [TestMethod]
        public void TestNativeParameters()
        {
            Util.AssertEventAttributes<TestsEventSource>("Parameters", 4, EventLevel.Informational, null, EventKeywords.None, EventTask.None, null);
            //DateTime and byte[] are not tested due to EventAnalyzer issues that should be fixed in the next release:
            //https://github.com/mspnp/semantic-logging/pull/83
            //https://github.com/mspnp/semantic-logging/pull/94
            m_testsEvents.Parameters(Guid.Empty, true, 'a', 1, 2, 3, 4, 5, 6, 7, 8, 1.0f, 2.3, "foo", Guid.NewGuid(), IntPtr.Zero);
        }

        [TestMethod]
        public void TestCustomTypes()
        {
            Util.AssertEventAttributes<TestsEventSource>("CustomTypes", 5, EventLevel.Critical, null, EventKeywords.None, EventTask.None, null);
            m_testsEvents.CustomTypes(Guid.Empty, new CustomType { Foo = "foo" }, 1.0, new AnotherCustomType(), TestEnum.Hello);
        }
    }
}