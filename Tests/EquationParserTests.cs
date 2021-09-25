using LinearEquationSolver;
using LinearEquationSolver.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class EquationParserTests
    {
        [TestMethod]
        public void GeneralValidCase()
        {
            // Arrange:
            string input = $"-345/7x + y12 = -8y - 9/5";
            ILinearEquationParser ep = new BasicLinearEquation();
            
            // Act:
            LinearEquation got =  ep.Parse(input);
            
            // Verify:
            LinearEquation want = new LinearEquation();
            want.AddTerm(new Term(new Fraction(20, 1), "y"));
            want.AddTerm(new Term(new Fraction(-345, 7), "x"));
            want.AddTerm(new Term(new Fraction(9, 5), ""));

            Assert.IsTrue(want == got);
            Assert.AreEqual(want.ToString(), got.ToString());
        }

        [TestMethod]
        public void SingleNumberInput()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();

            // Act:
            LinearEquation got = ep.Parse("10");

            // Verify:
            LinearEquation want = new LinearEquation();
            want.AddTerm(new Term(new Fraction(10, 1), ""));

            Assert.IsTrue(want == got);
            Assert.AreEqual(want.ToString(), got.ToString());
        }

        [TestMethod]
        public void SingleNegativeNumberInput()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();

            // Act:
            LinearEquation got = ep.Parse("-5");

            // Verify:
            LinearEquation want = new LinearEquation();
            want.AddTerm(new Term(new Fraction(-5, 1), ""));

            Assert.IsTrue(want == got);
            Assert.AreEqual(want.ToString(), got.ToString());
        }

        [TestMethod]
        public void EmptyInput()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();
            string[] testCases = new string[] { "",  "\t\t\t\t\t", "\t\n\t", "     ", " \n \n\n\n\t", " \n  \t ", " \n "};
            foreach (var t in testCases)
            {
                // Act:
                LinearEquation got = ep.Parse(t);
                // Verify:
                LinearEquation want = new LinearEquation();
                Assert.IsTrue(want == got);
                Assert.AreEqual(0, got.GetTerms().Count());
                Assert.AreEqual(want.ToString(), got.ToString());
            }
        }

        [TestMethod]
        public void OnlyOneValidTermNoEquals()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();

            // Act:
            LinearEquation got = ep.Parse("-5/3x");

            // Verify:
            LinearEquation want = new LinearEquation();
            want.AddTerm(new Term(new Fraction(-5, 3), "x"));

            Assert.IsTrue(want == got);
            Assert.AreEqual(want.ToString(), got.ToString());
        }

        [TestMethod]
        public void FractionsWithVariableDenominator()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();
            var testCases = new[] {
                new
                {
                    Input = "-9/x",
                    Want = new LinearEquation(new Term(new Fraction(-9, 1), "x"))
                },
                new
                {
                    Input = "x/-2",
                    Want = new LinearEquation(new Term(new Fraction(1, -2), "x"))
                },
                new
                {
                    Input = "longer/99999",
                    Want = new LinearEquation(new Term(new Fraction(1, 99999), "longer"))
                },
                new
                {
                    Input = "-x/3 + y/-2 - -9/-1 + -80    / z = 0", // This input is "kinda" illigal ..
                    Want = new LinearEquation(
                                new Term(new Fraction(-1, 3), "x"),
                                new Term(new Fraction(1, -2), "y"),
                                new Term(new Fraction(-80, 1), "z"),
                                new Term(-new Fraction(-9, -1), "")
                            )
                },
            };

            foreach (var t in testCases)
            {
                // Act
                LinearEquation got = ep.Parse(t.Input);
                // Assert:
                LinearEquation want = t.Want;
                Assert.IsTrue(want == got, $"{t.Input} test failed");
                Assert.AreEqual(want.ToString(), got.ToString(), $"{t.Input} test failed");
            }
        }

        [TestMethod]
        public void ExceptionalCases()
        {
            // Arrange:
            ILinearEquationParser ep = new BasicLinearEquation();
            var testCases = new[] {
                new
                {
                    Input = "8x9",
                    Want = new FormatException("Term format is invalid")
                },
                new
                {
                    Input = "-8x9",
                    Want = new FormatException("Term format is invalid")
                },
                new
                {
                    Input = "3x + 12y8",
                    Want = new FormatException("Term format is invalid")
                },
                new
                {
                    Input = "12y*",
                    Want = new FormatException("Term format is invalid")
                },
                new
                {
                    Input = "//3",
                    Want = new FormatException("Term format is invalid: //3")
                },
                new
                {
                    Input = "4/8/x",
                    Want = new FormatException("Term format is invalid: 4/8/x")
                },
            };

            foreach (var t in testCases)
            {
                // Act
                var ex = Assert.ThrowsException<FormatException>(() => ep.Parse(t.Input));
                // Assert:
                FormatException want = t.Want;
                Assert.AreEqual(ex.Message, want.Message);
            }
        }
    }
}
