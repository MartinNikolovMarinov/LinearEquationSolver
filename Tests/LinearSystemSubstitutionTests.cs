using LinearEquationSolver;
using LinearEquationSolver.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class LinearSystemSubstitutionTests
    {
        [TestMethod]
        public void ValidEquationTests()
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    Input = new string[]
                    {
                        "x = 1",
                        "-5y + 2z = -5",
                        "y + 2x = 11",
                    },
                    WantSubs = 1,
                    Want = new Dictionary<string, Fraction>{
                        { "x", new Fraction(1, 1) },
                        { "y", new Fraction(9, 1) },
                        { "z", new Fraction(20, 1) },
                    },
                },
                new
                {
                    Input = new string[]
                    {
                        "x + y - z = 11",
                        "a - z + y = 9",
                        "b - z + a = 11",
                        "x + b + y + z = 23",
                        "b = 8",
                        "a = 5",
                        "d = 90"
                    },
                    WantSubs = 3,
                    Want = new Dictionary<string, Fraction>{
                        { "a", new Fraction(5, 1) },
                        { "b", new Fraction(8, 1) },
                        { "d", new Fraction(90, 1) },
                        { "x", new Fraction(7, 1) },
                        { "y", new Fraction(6, 1) },
                        { "z", new Fraction(2, 1) },
                    },
                },
                new
                {
                    Input = new string[]
                    {
                        "a = 1",
                        "b = 2",
                        "c = 10",
                    },
                    WantSubs = 0,
                    Want = new Dictionary<string, Fraction>{
                        { "a", new Fraction(1, 1) },
                        { "b", new Fraction(2, 1) },
                        { "c", new Fraction(10, 1) },
                    },
                },
            };

            foreach (var t in testCases)
            {
                LinearEquation[] eqs = t.Input.Select(x => lep.Parse(x)).ToArray();
                LinearSystem ls = new LinearSystem(eqs);
                long wantSubs = t.WantSubs;

                // Assert that the exact wanted number of substitutions happened and NO more.
                LinearSystem.SubstitutionResult subResult;
                while (wantSubs > 0)
                {
                    subResult = ls.SubstituteEquations();
                    Assert.IsTrue(subResult == LinearSystem.SubstitutionResult.SUBSTITUTION_OCCURRED);
                    wantSubs--;
                }
                subResult = ls.SubstituteEquations();
                Assert.IsTrue(subResult == LinearSystem.SubstitutionResult.NO_SUBSTITUTION_OCCURRED);

                // Check resulting system:
                Assert.IsTrue(ls.Equations.TrueForAll(x => x.HasSolution()));
                Assert.IsTrue(ls.Equations.TrueForAll(x =>
                {
                    LinearEquation.Solution s = x.GetSolution();
                    bool res = false;
                    if (t.Want.ContainsKey(s.Variable))
                    {
                        Fraction val = t.Want[s.Variable];
                        res = (val == s.Value);
                    }

                    return res;
                }));
            }
        }

        [TestMethod]
        public void PartOfTheSystemCanBeSubstituted()
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            string[] input = new string[] {
                "x + b + y + z = 0",
                "b = 8",
                "z = -8",
            };
            
            LinearEquation[] eqs = input.Select(x => lep.Parse(x)).ToArray();
            LinearSystem ls = new LinearSystem(eqs);

            LinearSystem.SubstitutionResult subResult = ls.SubstituteEquations();
            Assert.IsTrue(subResult == LinearSystem.SubstitutionResult.SUBSTITUTION_OCCURRED);
            subResult = ls.SubstituteEquations();
            Assert.IsTrue(subResult == LinearSystem.SubstitutionResult.NO_SUBSTITUTION_OCCURRED);

            Assert.AreEqual("y + x = 0", ls.Equations[0].ToString());
            Assert.AreEqual("b = 8", ls.Equations[1].ToString());
            Assert.AreEqual("z = -8", ls.Equations[2].ToString());
        }

        [TestMethod]
        public void ContradictoryAndNoSolutionEquationTests()
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    Input = new string[]
                    {
                        "x = 2",
                        "x = 10",
                    },
                    Want = LinearSystem.SubstitutionResult.CONTRADICTION,
                },
                new
                {
                    Input = new string[]
                    {
                        "0 = 4",
                    },
                    Want = LinearSystem.SubstitutionResult.CONTRADICTION,
                },
                new
                {
                    Input = new string[]
                    {
                        "x + y = 0",
                        "z + x = 0",
                    },
                    Want = LinearSystem.SubstitutionResult.NO_SUBSTITUTION_OCCURRED,
                },
            };

            foreach (var t in testCases)
            {
                LinearEquation[] eqs = t.Input.Select(x => lep.Parse(x)).ToArray();
                LinearSystem ls = new LinearSystem(eqs);

                LinearSystem.SubstitutionResult subResult = ls.SubstituteEquations();
                Assert.AreEqual(t.Want, subResult);
            }
        }
    }
}
