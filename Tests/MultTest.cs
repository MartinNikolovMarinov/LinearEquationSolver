using LinearEquationSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MultTest
    {
        [TestMethod]
        public void SimpleMult()
        {
            // Arrange
            Fraction f1 = new Fraction(3, 9);
            Fraction f2 = new Fraction(4, 7);

            Assert.AreEqual(new Fraction(3 * 4, 9 * 7), f1 * f2);
            Assert.AreEqual(new Fraction(3, 9), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(4, 7), f2, "Fraction did not remain the same");
        }
    }
}
