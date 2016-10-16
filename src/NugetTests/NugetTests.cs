using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NugetTests
{
    [TestClass]
    public class NugetTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            new NugetEvents().Foo();
        }
    }
}
