using LinearEquationSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CompareTests
    {
        [TestMethod]
        public void CompareTest()
        {
            // Arrange
            Fraction f1 = new Fraction(-6, 9);
            Fraction f2 = new Fraction(6, 9);

            Assert.AreEqual(true, f1 < f2);
            Assert.AreEqual(true, f1 <= f2);
            Assert.AreEqual(false, f1 > f2);
            Assert.AreEqual(false, f1 >= f2);

            Assert.AreEqual(-1, f1.CompareTo(f2));

            f1 *= -Fraction.One;
            Assert.AreEqual(false, f1 > f2);
            Assert.AreEqual(true, f1 >= f2);
            Assert.AreEqual(true, f1 == f2);
            Assert.AreEqual(false, f1 < f2);
            Assert.AreEqual(true, f1 <= f2);

            Assert.AreEqual(0, f1.CompareTo(f2));
        }
    }
}
