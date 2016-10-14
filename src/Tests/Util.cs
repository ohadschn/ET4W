using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Tracing;

namespace Tests
{
    public static class Util
    {
        public static void AssertEventAttributes<T>(string eventName, int expectedId, EventLevel expectedLevel, string expectedMessage,
            EventKeywords expectedKeywords, EventTask expectedTask, EventOpcode? expectedOpcode)
        {
            var eventAttribute = typeof(T).GetMethod(eventName).GetCustomAttribute<EventAttribute>();
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
    }
}
