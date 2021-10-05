using LinearEquationSolver;
using LinearEquationSolver.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class LinearEquationSimpleTests
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

            // Validate that fraction simplification works with removing zero terms:
            for (int i = 1; i < 10; i++)
            {
                l.AddTerm(new Term(-new Fraction(1, 2), "x"));
                l.AddTerm(new Term(new Fraction(1*i, 2*i), "x"));
            }
            CollectionAssert.AreEqual(new Term[] { }, l.GetTerms().ToArray());
            Assert.AreEqual("", l.ToString());

            l = new LinearEquation();
            // Only constant terms should not produce equations strings:
            var emptyStuff = new string[10] { null, "", " ", "   ", "\t", "\n", "\r\n", " \t", "\t \t", "\t\t\t\t\t\n\n\n\n\n\r\n\r\n\t   \n\t" };
            for (int i = 0; i < 10; i++)
            {
                l.AddTerm(new Term(new Fraction(i, 1), emptyStuff[i]));
            }
            CollectionAssert.AreEqual(new Term[] { new Term((Fraction)45, "") }, l.GetTerms().ToArray());
            Assert.AreEqual("45 = 0", l.ToString());
        }

        [TestMethod]
        public void SpecialCasesWithEquals()
        {
            var l = new LinearEquation(new Term(-new Fraction(1, 2), "x"));
            Assert.IsFalse(l.Equals(new Term(-new Fraction(1, 2), "x")));
            Assert.IsFalse(l.Equals(10));
            Assert.IsTrue(l.Equals(l));
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

        [TestMethod]
        public void ComparingLinearEquations()
        {
            ILinearEquationParser ep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    A = "5y + 3x = 0",
                    B = "4y + 3x = 0",
                    Want = 1,
                    Msg = "Leftmost term is greater in A"
                },
                new
                {
                    A = "5y + 3x = 0",
                    B = "6y + 3x = 0",
                    Want = -1,
                    Msg = "leftmost term is smaller in A"
                },
                new
                {
                    A = "2z + 6y + 7y = 12",
                    B = "2z + 6y + 7y = 12",
                    Want = 0,
                    Msg = "A and B are equal"
                },
                new
                {
                    A = "2z + 6y + 7y = 12",
                    B = "2z + 6y = 12",
                    Want = 1,
                    Msg = "A has more terms then B"
                },
                new
                {
                    A = "2z + 6y = 12",
                    B = "2z + 6y + 7y = 12",
                    Want = -1,
                    Msg = "B has more terms then A"
                },
                new
                {
                    A = "2z + 6y + 7x = 12",
                    B = "2z + 6y + 8s = 3",
                    Want = 1,
                    Msg = "B does not have an x term so A is in front"
                },
                new
                {
                    A = "2z + 6y + 8s = 3",
                    B = "2z + 6y + 7x = 12",
                    Want = -1,
                    Msg = "Same as above but A and B are reversed"
                },
                new
                {
                    A = "3y - 2y = 12",
                    B = "y = 12",
                    Want = 0,
                    Msg = "Default to comparing coefficients"
                },
                new
                {
                    A = "2",
                    B = "y = 12",
                    Want = -1,
                    Msg = "A is not an equation, so B should be in front"
                },
                new
                {
                    A = "3x = 0",
                    B = "7",
                    Want = 1,
                    Msg = "B is not an equation, so A should be in front"
                },
                new
                {
                    A = "19",
                    B = "7",
                    Want = 1,
                    Msg = "Both are not equations but the constant in A is larger"
                },
                new
                {
                    A = "",
                    B = "12",
                    Want = -1,
                    Msg = "A is empty"
                },
                new
                {
                    A = "9",
                    B = "",
                    Want = 1,
                    Msg = "B is empty"
                },
                new
                {
                    A = "",
                    B = "",
                    Want = 0,
                    Msg = "Both are empty"
                },
            };

            foreach (var t in testCases)
            {
                // Act
                LinearEquation a = ep.Parse(t.A);
                LinearEquation b = ep.Parse(t.B);
                // Assert:
                Assert.AreEqual(t.Want, a.CompareTo(b), t.Msg);
            }
        }
    }
}
