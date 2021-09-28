using LinearEquationSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class LinearSystemSubstitutionTests
    {
        [TestMethod]
        public void SameDenominator()
        {
            // Arrange
            Fraction f1 = new Fraction(7, 6);
            Fraction f2 = new Fraction(5, 6);

            Assert.AreEqual(new Fraction(12, 6), f1 + f2);
            Assert.AreEqual(new Fraction(2, 1), f1 + f2);
            Assert.AreEqual(new Fraction(24, 12), f1 + f2);
            Assert.AreEqual(new Fraction(7, 6), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(5, 6), f2, "Fraction did not remain the same");
        }
    }
}
