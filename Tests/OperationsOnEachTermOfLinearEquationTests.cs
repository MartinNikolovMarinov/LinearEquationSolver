using LinearEquationSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class OperationsOnEachTermOfLinearEquationTests
    {
        [TestMethod]
        public void TermEquality()
        {
            var t1 = new Term(new Fraction(1, 2), "y");
            var t2 = new Term(new Fraction(1, 3), "x");
            var t3 = new Term(-new Fraction(1, 2), "z");
            var t4 = new Term((Fraction)(0), null);

            var l = new LinearEquation(t1, t2, t3, t4);

            l.MultEachTermBy((Fraction)2);

            var got = l.GetTerms().ToArray();
            var expected = new Term[] 
            {
                new Term(-Fraction.One, "z"),
                new Term(Fraction.One, "y"),
                new Term(new Fraction(2, 3), "x"),
            };

            CollectionAssert.AreEqual(expected, got);
            Assert.AreEqual("-z + y + 2/3 x = 0", l.ToString());
        }

        [TestMethod]
        public void SimplifyDenominators()
        {
            var t1 = new Term(new Fraction(1, 2), "z");
            var t2 = new Term(-new Fraction(1, 4), "y");
            var t3 = new Term(new Fraction(1, 8), "x");
            var t4 = new Term((Fraction)(0), null);

            var l = new LinearEquation(t1, t2, t3, t4);

            l.Simplify();

            var got = l.GetTerms().ToArray();
            var expected = new Term[]
            {
                new Term(Fraction.One, "z"),
                new Term(-new Fraction(1, 2), "y"),
                new Term(new Fraction(1, 4), "x"),
            };

            CollectionAssert.AreEqual(expected, got);
            Assert.AreEqual("z - 1/2 y + 1/4 x = 0", l.ToString());
        }


        [TestMethod]
        public void SimplifyNumberators()
        {
            var t1 = new Term((Fraction)2, "z");
            var t2 = new Term(new Fraction(4, 9), "y");
            var t3 = new Term((Fraction)(-8), "x");

            var l = new LinearEquation(t1, t2, t3);

            l.Simplify();

            var got = l.GetTerms().ToArray();
            var expected = new Term[]
            {
                new Term(Fraction.One, "z"),
                new Term(new Fraction(2, 9), "y"),
                new Term((Fraction)(-4), "x"),
            };

            CollectionAssert.AreEqual(expected, got);
            Assert.AreEqual("z + 2/9 y - 4 x = 0", l.ToString());
        }

        [TestMethod]
        public void SimplifyBoth()
        {
            var t1 = new Term(new Fraction(4, 9), "x");
            var t2 = new Term(new Fraction(8, 3), "y");
            var t3 = new Term(-new Fraction(10, 81), "");

            var l = new LinearEquation(t1, t2, t3);

            l.Simplify();

            var got = l.GetTerms().ToArray();
            var expected = new Term[]
            {
                new Term(new Fraction(4, 1), "y"),
                new Term(new Fraction(2, 3), "x"),
                new Term(-new Fraction(5, 27), ""),
            };

            CollectionAssert.AreEqual(expected, got);
            Assert.AreEqual("4 y + 2/3 x = 5/27", l.ToString());
        }
    }
}
