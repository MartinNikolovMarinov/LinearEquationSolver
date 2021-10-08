using LinearEquationSolver;
using LinearEquationSolver.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class LinearEquationExpressVariableTests
    {
        [TestMethod]
        public void ExpressVariableWithCorrectInput()
        {
            ILinearEquationParser ep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    Equation = "3a + 5b + 3/2c = 0",
                    Variable = "a",
                    Want = new Term[] {
                         new Term(-new Fraction(1, 2), "c"),
                         new Term(-new Fraction(5, 3), "b"),
                    },
                },
                new
                {
                    Equation = "3a + 5b + 3/2c = 0",
                    Variable = "b",
                    Want = new Term[] {
                         new Term(-new Fraction(3, 10), "c"),
                         new Term(-new Fraction(3, 5), "a"),
                    },
                },
                new
                {
                    Equation = "3a + 5b + 3/2c = 0",
                    Variable = "c",
                    Want = new Term[] {
                         new Term(-new Fraction(10, 3), "b"),
                         new Term((Fraction)(-2), "a"),
                    },
                },
                new
                {
                    Equation = "-5/6 p = 8 z + t/3 - 19",
                    Variable = "p",
                    Want = new Term[] {
                         new Term(-new Fraction(48, 5), "z"),
                         new Term(-new Fraction(2, 5), "t"),
                         new Term(new Fraction(114, 5), ""),
                    },
                },
                new
                {
                    Equation = "2a = 5 - 5",
                    Variable = "a",
                    Want = new Term[0],
                },
                new
                {
                    Equation = "2a = 6 - 5 + 7z - 7z",
                    Variable = "a",
                    Want = new Term[] {
                        new Term(new Fraction(1, 2), ""),
                    },
                },
            };

            foreach (var t in testCases)
            {
                // Act
                LinearEquation eq = ep.Parse(t.Equation);
                var got = eq.ExpressVariable(t.Variable);

                // Assert:
                CollectionAssert.AreEqual(t.Want, got);
            }
        }
    }
}
