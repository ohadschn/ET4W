using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NugetTests
{
    [TestClass]
    public class NugetTests
    {
        [TestMethod]
        public void AnalyzeNugetEvents()
        {
            EventSourceAnalyzer.InspectAll(NugetEventSource.Log);
        }

        [TestMethod]
        public void TestNugetEvents()
        {
            new NugetEvents().Foo();
        }
    }
}
