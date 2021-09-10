namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class TermTests
    {
        [TestMethod]
        public void TermEquality()
        {
            // Variable is different:
            Assert.IsTrue(new Term(new Fraction(1, 2), "x") != new Term(new Fraction(1, 2), "y"));
            Assert.IsTrue((new Term(new Fraction(1, 2), "x") == new Term(new Fraction(1, 2), "y")) == false);

            // Are the same:
            Assert.IsTrue(new Term(new Fraction(1, 2), "x") == new Term(new Fraction(1, 2), "x"));
            Assert.IsTrue((new Term(new Fraction(1, 2), "x") != new Term(new Fraction(1, 2), "x")) == false);
        }

        [TestMethod]
        public void TermSorting()
        {
            var input = new Term[] {
                new Term(new Fraction(long.MaxValue, long.MaxValue), "z"),
                new Term(new Fraction(1, 2), "x"),
                new Term(new Fraction(long.MinValue, long.MinValue), "y"),
                new Term(new Fraction(0, 0), ""),
            };

            var expected = new Term[] {
                new Term(new Fraction(0, 0), ""),
                new Term(new Fraction(1, 2), "x"),
                new Term(new Fraction(long.MinValue, long.MinValue), "y"),
                new Term(new Fraction(long.MaxValue, long.MaxValue), "z"),
            };

            Array.Sort(input);

            CollectionAssert.AreEqual(expected, input);
        }

        [TestMethod]
        public void TermSortingInDesc()
        {
            var input = new Term[] {
                new Term(new Fraction(long.MaxValue, long.MaxValue), "z"),
                new Term(new Fraction(1, 2), "x"),
                new Term(new Fraction(long.MinValue, long.MinValue), "y"),
                new Term(new Fraction(0, 0), ""),
            };

            var expected = new Term[] {
                new Term(new Fraction(long.MaxValue, long.MaxValue), "z"),
                new Term(new Fraction(long.MinValue, long.MinValue), "y"),
                new Term(new Fraction(1, 2), "x"),
                new Term(new Fraction(0, 0), ""),
            };

            Array.Sort(input, (a, b) => b.CompareTo(a));

            CollectionAssert.AreEqual(expected, input);
        }
    }
}
