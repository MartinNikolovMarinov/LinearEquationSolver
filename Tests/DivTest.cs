namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DivTest
    {
        [TestMethod]
        public void SimpleDiv()
        {
            // Arrange
            Fraction f1 = new Fraction(5, 9);
            Fraction f2 = new Fraction(9, 3);

            Assert.AreEqual(new Fraction(5 * 3, 9 * 9), f1 / f2);
            Assert.AreEqual(new Fraction(5, 9), f1, "Fraction did not remain the same");
            Assert.AreEqual(new Fraction(9, 3), f2, "Fraction did not remain the same");
        }
    }
}
