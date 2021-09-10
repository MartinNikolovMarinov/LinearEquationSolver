namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubTests
    {
        [TestMethod]
        public void SimpleSub()
        {
            // Arrange
            Fraction f1 = new Fraction(1, 2);
            Fraction f2 = new Fraction(2, 1);

            Assert.AreEqual(-new Fraction(3, 2), f1 - f2);
            Assert.AreEqual(-new Fraction(3 * 412, 2 * 412), f1 - f2);
            Assert.AreEqual(new Fraction(1, 2), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(2, 1), f2, "Fraction did not remain the same");
        }

        [TestMethod]
        public void SubtactingTheSameNumber()
        {
            // Arrange
            Fraction f1 = new Fraction(7, 7);
            Fraction f2 = new Fraction(3, 3);

            Assert.AreEqual(new Fraction(0, 0), f1 - f2);
            Assert.AreEqual(new Fraction(7, 7), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(3, 3), f2, "Fraction did not remain the same");
        }
    }
}
