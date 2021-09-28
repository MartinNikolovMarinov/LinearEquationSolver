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
        public void SolvingValidEquations()
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
                }
            };

            foreach (var t in testCases)
            {
                LinearEquation[] eqs = t.Input.Select(x => lep.Parse(x)).ToArray();
                LinearSystem ls = new LinearSystem(eqs);
                ls.Solve();

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
    }
}
