namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AddTests
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

        [TestMethod]
        public void DivisibleDenomerators()
        {
            // Arrange
            Fraction f1 = new Fraction(1, 2);
            Fraction f2 = new Fraction(5, 6);

            Assert.AreEqual(new Fraction(16, 12), f1 + f2);
            Assert.AreEqual(new Fraction(4, 3), f1 + f2);
            Assert.AreEqual(new Fraction(1, 2), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(5, 6), f2, "Fraction did not remain the same");
        }

        [TestMethod]
        public void NotDivisibleDenomerators()
        {
            // Arrange
            Fraction f1 = new Fraction(3, 7);
            Fraction f2 = new Fraction(8, 3);

            Assert.AreEqual(new Fraction(65, 21), f1 + f2);
            Assert.AreEqual(new Fraction(3, 7), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(8, 3), f2, "Fraction did not remain the same");
        }
    }
}
