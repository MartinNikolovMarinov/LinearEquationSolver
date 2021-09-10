
namespace Tests
{
    using System;
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestsWithZero
    {
        [TestMethod]
        public void DivByZero()
        {
            // Arrange
            Fraction f1 = new Fraction(1, 2);
            Fraction f2 = Fraction.Zero;

            Assert.AreEqual(Fraction.Zero, (f1 / f2)); // this is a wierd case (1/2) / (0/0) = (1/2) * (0/0) = 0/0
            Assert.ThrowsException<InvalidOperationException>(() => new Fraction(12, 0));
        }

        [TestMethod]
        public void MultByZero()
        {
            // Arrange
            Fraction f1 = new Fraction(long.MaxValue, long.MaxValue);
            Fraction f2 = Fraction.Zero;

            Assert.AreEqual(Fraction.Zero, f1 * f2);
        }

        [TestMethod]
        public void AdditionWithZero()
        {
            // Arrange
            Fraction f1 = new Fraction(long.MaxValue, long.MaxValue);
            Fraction f2 = new Fraction(0, long.MaxValue);

            Assert.AreEqual(new Fraction(long.MaxValue, long.MaxValue), f1 + f2);
        }

        [TestMethod]
        public void OperationsWithOne()
        {
            Assert.AreEqual(new Fraction(5, 3), Fraction.One * new Fraction(5, 3));
            Assert.AreEqual(new Fraction(3, 5), Fraction.One / new Fraction(5, 3));
            Assert.AreEqual(new Fraction(8, 3), Fraction.One + new Fraction(5, 3));
            Assert.AreEqual(-new Fraction(2, 3), Fraction.One - new Fraction(5, 3));
        }

        [TestMethod]
        public void OperationsWithOneAndZero()
        {
            Assert.AreEqual(new Fraction(0, 0), Fraction.One * Fraction.Zero);
            Assert.AreEqual(Fraction.Zero, Fraction.One / Fraction.Zero); // this is a wierd case (1/1) / (0/0) = (1/1) * (0/0) = 0/0
            Assert.AreEqual(Fraction.One, Fraction.One + Fraction.Zero);
            Assert.AreEqual(new Fraction(1, 1), Fraction.One + Fraction.Zero);
            Assert.AreEqual((Fraction)1, Fraction.One + Fraction.Zero);
            Assert.AreEqual(new Fraction(-1, -1), Fraction.One + Fraction.Zero);
            Assert.AreEqual(new Fraction(Fraction.One), Fraction.One + Fraction.Zero);

            Assert.AreEqual(Fraction.One, Fraction.One - Fraction.Zero);
            Assert.AreEqual(-Fraction.One, Fraction.Zero - Fraction.One);
            Assert.AreEqual((Fraction)1, Fraction.One - Fraction.Zero);
            Assert.AreEqual((Fraction)(-1), Fraction.Zero - Fraction.One);
            Assert.AreEqual((Fraction)(0), Fraction.Zero - Fraction.Zero);
            Assert.AreEqual(new Fraction(-1, 1), Fraction.Zero - Fraction.Zero - Fraction.One);
            Assert.AreEqual(new Fraction(1, -1), Fraction.Zero - Fraction.Zero - Fraction.One);
            Assert.AreEqual(new Fraction(-Fraction.One), Fraction.Zero - Fraction.Zero - Fraction.One);
        }
    }
}
