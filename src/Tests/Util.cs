using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Tracing;
using System.Collections.Generic;

namespace Tests
{
    public static class Util
    {
        public static void AssertEventAttributes<T>(string eventName, int expectedId, EventLevel expectedLevel, string expectedMessage,
            EventKeywords expectedKeywords, EventTask expectedTask, EventOpcode? expectedOpcode)
        {
            var eventAttribute = typeof(T).GetMethod(eventName).GetCustomAttribute<EventAttribute>();
            Assert.AreEqual(expectedId, eventAttribute.EventId, "Mismatched attribute event ID");
            Assert.AreEqual(expectedLevel, eventAttribute.Level, "Mismatched attribute event level");
            Assert.AreEqual(expectedMessage, eventAttribute.Message, "Mismatched attribute event message");
            Assert.AreEqual(expectedKeywords, eventAttribute.Keywords, "Mismatched attribute event keywords");
            Assert.AreEqual(expectedTask, eventAttribute.Task, "Mismatched attribute event task");

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

        public static void AssertEventRecord(this MemoryRetentionSink sink, 
            int expectedId, string expectedFormattedMessage, IList<object> expectedPayload, DateTimeOffset expectedTimestamp = default(DateTimeOffset))
        {
            if (expectedTimestamp == default(DateTimeOffset))
            {
                expectedTimestamp = DateTimeOffset.UtcNow;
            }

            EventRecord record;
            bool recordTaken = sink.RetainedEvents.TryTake(out record);
            Assert.IsTrue(recordTaken, "Could not take event from sink bag");
            Assert.AreEqual(0, sink.RetainedEvents.Count, "More than one event present in the sink");

            Assert.AreEqual(expectedId, record.Id, "Mismatched sink event ID");
            Assert.AreEqual(expectedFormattedMessage, record.FormattedMessage, "Mismatched sink formatted message");

            foreach (var tup in expectedPayload.Zip(record.Payload, Tuple.Create))
            {
                Assert.AreEqual(tup.Item1, tup.Item2, "Mismatched sink payload object");
            }

            Assert.IsTrue((expectedTimestamp - record.Timestamp) < TimeSpan.FromSeconds(5), 
                $"Mismatched sink timestamp (more than 5 seconds apart). Expected: {expectedTimestamp}; Actual: {record.Timestamp}");

            Assert.AreEqual(0, sink.RetainedErrors.Count, "Sink errors detected: " + String.Join(", ", sink.RetainedErrors));
        }
    }
}
