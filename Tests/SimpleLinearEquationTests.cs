namespace Tests
{
    using LinearEquationSolver;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class SimpleLinearEquationTests
    {
        [TestMethod]
        public void GeneralCaseInit()
        {
            var t1 = new Term(-new Fraction(1, 2), "z");
            var t2 = new Term(new Fraction(1, 2), "y");
            var t3 = new Term(new Fraction(1, 3), "x");
            var t4 = new Term(-Fraction.One, "x"); // Should merge with t3
            var t5 = new Term((Fraction)(-2), null);

            var l = new LinearEquation(t1, t2, t3);
            l.AddTerm(t4);
            l.AddTerm(t5);

            l.AddTerm(new Term(Fraction.Zero, "should be gone"));
            l.AddTerm(new Term(new Fraction(0, 0), "should be gone"));
            l.AddTerm(new Term(new Fraction(0, 7), "should be gone"));
            l.AddTerm(new Term(new Fraction(0, -1237), "should be gone"));

            var want = new Term[] {
                new Term(-new Fraction(1, 2), "z"),
                new Term(new Fraction(1, 2), "y"),
                new Term(-new Fraction(2, 3), "x"),
                new Term((Fraction)(-2), null),
            };
            var got = l.GetTerms().ToArray();
            string fracAsStr = l.ToString();
            string expectedFracStr = "-1/2 z + 1/2 y - 2/3 x = 2";

            CollectionAssert.AreEqual(want, got);
            Assert.AreEqual(fracAsStr, expectedFracStr);
        }

        [TestMethod]
        public void EdgeCasesWithZeroes()
        {
            var l = new LinearEquation();

            CollectionAssert.AreEqual(new Term[] { }, l.GetTerms().ToArray());
            Assert.AreEqual("", l.ToString());

            // Validate that fraction simplification works with removeing zero terms:
            for (int i = 1; i < 10; i++)
            {
                l.AddTerm(new Term(-new Fraction(1, 2), "x"));
                l.AddTerm(new Term(new Fraction(1*i, 2*i), "x"));
            }
            CollectionAssert.AreEqual(new Term[] { }, l.GetTerms().ToArray());
            Assert.AreEqual("", l.ToString());

            // Only constant terms should not produce equations strings:
            var emptyStuff = new string[10] { null, "", " ", "   ", "\t", "\n", "\r\n", " \t", "\t \t", "\t\t\t\t\t\n\n\n\n\n\r\n\r\n\t   \n\t" };
            for (int i = 0; i < 10; i++)
            {
                l.AddTerm(new Term(new Fraction(i, 1), emptyStuff[i]));
            }
            CollectionAssert.AreEqual(new Term[] { new Term((Fraction)45, "") }, l.GetTerms().ToArray());
            Assert.AreEqual("", l.ToString());
        }

        [TestMethod]
        public void OnlyOneTerm()
        {
            var l = new LinearEquation(new Term(-new Fraction(1, 2), "я"));

            var want = new Term[] {
                new Term(-new Fraction(1, 2), "я"),
            };
            var got = l.GetTerms().ToArray();
            string fracAsStr = l.ToString();
            string expectedFracStr = "-1/2 я = 0";

            CollectionAssert.AreEqual(want, got);
            Assert.AreEqual(fracAsStr, expectedFracStr);
        }
    }
}
