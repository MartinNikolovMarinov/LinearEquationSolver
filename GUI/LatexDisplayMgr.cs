using LinearEquationSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GUI
{
    public static class LatexDisplayMgr
    {
        public static bool SolutionToLatex(StringBuilder latexBuf, LinearSystem ls)
        {
            bool allHaveSolution = ls.Equations.TrueForAll(x => x.HasSolution());
            if (allHaveSolution && ls.Equations.Count > 0)
            {
                latexBuf.Append(@"\begin{array}{rcc}");
                HashSet<string> uniqueVars = new HashSet<string>();
                for (int i = 0; i < ls.Equations.Count; i++)
                {
                    LinearEquation curr = ls.Equations[i];
                    LinearEquation.Solution solution = curr.GetSolution();
#if DEBUG
                    Debug.Assert(solution != null); // sanity check.
#endif
                    if (solution.Variable == "0") continue; // skip cases like 0 = 0
                    
                    if (uniqueVars.Add(solution.Variable))
                    {
                        latexBuf.Append($"{solution.Variable} &=& {solution.Value}");
                        latexBuf.AppendLine(@"\\");
                    }
                }

                latexBuf.Append(@"\end{array}");
                return true;
            }

            return false;
        }

        public static bool LinearSystemToLatex(StringBuilder latexBuf, LinearSystem ls, int srcIndex = -1, int destIndex = -1)
        {
            if (ls.Equations.Count > 0)
            {
                
                latexBuf.Append(@"\begin{array}{c}");
                int i = 0;
                foreach (LinearEquation eq in ls.Equations)
                {
                    string eqStr = eq.ToString();

                    if (i == destIndex) eqStr = $"\\color{{red}}{{{eqStr}}}";
                    if (i == srcIndex) eqStr = $"\\color{{blue}}{{{eqStr}}}";

                    latexBuf.Append(eqStr);
                    latexBuf.Append($" \\quad p_{{{i++}}}");
                    latexBuf.AppendLine(@"\\");
                }

                latexBuf.Append(@"\end{array}");
                latexBuf.AppendLine(@"\\");
                return true;
            }

            return false;
        }
    }
}
