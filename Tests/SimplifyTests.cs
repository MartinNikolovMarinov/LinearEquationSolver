namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SimplifyTests
    {
        [TestMethod]
        public void SimplifyTest()
        {
            // Arrange
            Fraction f01 = new Fraction(0, 0);
            Fraction f02 = new Fraction(0, 18);
            Fraction f1 = new Fraction(1, 2);
            Fraction f2 = new Fraction(5, 6);
            Fraction f3 = new Fraction(2, 8);
            Fraction f4 = new Fraction(18, 3);
            Fraction f5 = new Fraction(-21, 7);
            Fraction f6 = new Fraction(-12, -6);
            Fraction f7 = new Fraction(99, -9);

            Assert.AreEqual(f01, f02);
            Assert.AreEqual(Fraction.Zero, f01);
            Assert.AreEqual(Fraction.Zero, f02);

            Assert.AreEqual(new Fraction(1, 2), f1);
            Assert.AreEqual(new Fraction(-1, -2), f1);
            Assert.AreEqual(new Fraction(5, 6), f2);
            Assert.AreEqual(new Fraction(1, 4), f3);
            Assert.AreEqual(new Fraction(6, 1), f4);
            Assert.AreEqual(new Fraction(-3, 1), f5);
            Assert.AreEqual(new Fraction(-2, -1), f6);
            Assert.AreEqual(new Fraction(2, 1), f6);
            Assert.AreEqual(new Fraction(11, -1), f7);
            Assert.AreEqual(new Fraction(-11, 1), f7);
        }
    }
}
