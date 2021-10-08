using LinearEquationSolver;
using LinearEquationSolver.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class LinearSystemSolveTests
    {
        [TestMethod]
        public void SolveEquationsWithSingleSolution()
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    Input = new string[]
                    {
                        "x + y = 0",
                        "2x - y + 3z = 3",
                        "x - 2y - z = 3",
                    },
                    Want = new Dictionary<string, Fraction>{
                        { "x", Fraction.One },
                        { "y", -Fraction.One },
                        { "z", Fraction.Zero },
                    },
                },
                new
                {
                    Input = new string[]
                    {
                        "2x + 3y = 13",
                        "x - y = -1",
                    },
                    Want = new Dictionary<string, Fraction>{
                        { "x", new Fraction(2) },
                        { "y", new Fraction(3) },
                    },
                },
            };

            foreach (var t in testCases)
            {
                LinearEquation[] eqs = t.Input.Select(x => lep.Parse(x)).ToArray();
                LinearSystem ls = new LinearSystem(eqs);
                ls.DirectSolve();

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
        public void SolvingValidEquationsWithParameterizedSolution()
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            var testCases = new[] {
                new
                {
                    Input = new string[]
                    {
                        "x + y + z - w = 1",
                        "y - z + w = -1",
                        "3x + 6z - 6w = 6",
                        "- y + z - w = 1",
                    },
                    Comparison = ComparisonFactory.CreateReverseComparison(),
                    Want = new LinearEquation[]{
                        lep.Parse("z + y + x - w = 1"),
                        lep.Parse("2y + x = 0"),
                        lep.Parse("0 = 0"),
                        lep.Parse("0 = 0"),
                    },
                },
                new
                {
                    Input = new string[]
                    {
                        "x + y + z - w = 1",
                        "y - z + w = -1",
                        "3x + 6z - 6w = 6",
                        "- y + z - w = 1",
                    },
                    Comparison = ComparisonFactory.CreateVariableOrderComparison(new Dictionary<string, int>() {
                        { "x", 5 },
                        { "y", 4 },
                        { "z", 3 },
                        { "w", 2 },
                    }),
                    Want = new LinearEquation[]{
                        lep.Parse("x + y + z - w = 1"),
                        lep.Parse("y - z + w = -1"),
                        lep.Parse("0 = 0"),
                        lep.Parse("0 = 0"),
                    },
                },
            };

            foreach (var t in testCases)
            {
                LinearEquation[] eqs = t.Input.Select(x => lep.Parse(x)).ToArray();
                LinearSystem ls = new LinearSystem(eqs);
                ls.SetSystemWideComparison(t.Comparison);
                foreach (var l in t.Want)
                {
                    l.CustomComparison = t.Comparison;
                }

                ls.DirectSolve();

                // Assert:
                Assert.IsFalse(ls.Equations.TrueForAll(x => x.HasSolution()));
                Assert.AreEqual(t.Want.Count(), ls.Equations.Count(), "Equation count mismatch");

                for (int i = 0; i < ls.Equations.Count(); i++)
                {
                    Term[] wantTerms = t.Want[i].GetTerms().ToArray();
                    Term[] gotTerms = ls.Equations[i].GetTerms().ToArray();
                    Assert.AreEqual(wantTerms.Length, gotTerms.Length, "Terms count mismatch");
                    for (int j = 0; j < wantTerms.Length; j++)
                    {
                        Assert.AreEqual(wantTerms[j], gotTerms[j]);
                    }
                }
            }
        }
    }
}
