using System;
using System.Collections.Generic;

namespace LinearEquationSolver
{
    public class LinearSystem
    {
        private List<LinearEquation> equations;

        public List<LinearEquation> Equations => this.equations;

        public LinearSystem(params LinearEquation[] eqs)
        {
            this.equations = new List<LinearEquation>(eqs);
        }

        public void AddEquation(LinearEquation eq)
        {
            this.equations.Add(eq);
        }

        public enum SubstitutionResult
        {
            NO_SUBSTITUTION_OCCURED,
            SUBSTITUTION_OCCURED,
            CONTRADICTION,
        }

        public SubstitutionResult SubstituteEquations()
        {
            SubstitutionResult result = SubstitutionResult.NO_SUBSTITUTION_OCCURED;
            for (int i = 0; i < this.equations.Count; i++)
            {
                LinearEquation curr = this.equations[i];
                LinearEquation.Solution currSolution = curr.GetSolution();
                if (currSolution != null)
                {
                    for (int j = 0; j < this.equations.Count; j++)
                    {
                        if (i == j) continue; // Skip self

                        LinearEquation next = this.equations[j];
                        LinearEquation.Solution nextSolution = next.GetSolution();
                        if (nextSolution != null)
                        {
                            if (currSolution.Variable == nextSolution.Variable && currSolution.Value != nextSolution.Value)
                            {
                                // Contradiction found
                                result = SubstitutionResult.CONTRADICTION;
                                goto DONE;
                            }
                        }
                        else
                        {
                            bool madeSubst = next.Substitute(currSolution.Variable, currSolution.Value);
                            if (madeSubst)
                            {
                                result = SubstitutionResult.SUBSTITUTION_OCCURED;
                            }
                        }
                    }
                }
            }

        DONE:
            return result;
        }

        // TODO: Split solution into self documenting steps and tidy up the mess !
        public void Solve()
        {
            bool done = false;
            while(!done)
            {
                done = true;
                for (int i = 0; i < this.equations.Count; i++)
                {
                    LinearEquation curr = this.equations[i];
                    Term currLeadingTerm = curr.GetLeadingTerm();
                    if (curr.HasSolution())
                    {
                        continue;
                    }

                    for (int j = i + 1; j < this.equations.Count; j++)
                    {
                        LinearEquation next = this.equations[j];
                        Term nextLeadingTerm = next.GetLeadingTerm();
                        if (next.HasSolution())
                        {
                            continue;
                        }

                        if (currLeadingTerm.Variable == nextLeadingTerm.Variable)
                        {
                            bool shouldBePositive = (currLeadingTerm.IsPositive() == nextLeadingTerm.IsPositive()) ? true : false;
                            Fraction x = currLeadingTerm.Coefficient / nextLeadingTerm.Coefficient;
                            if (!shouldBePositive)
                            {
                                x = -x;
                            }

                            LinearEquation steq = new LinearEquation(this.equations[i]);
                            steq.DivEachTermBy(x);
                            foreach (var stterm in steq.GetTerms())
                            {
                                this.equations[j].AddTerm(stterm);
                            }
                            this.equations[j].Simplify();

                            SubstitutionResult subResult = this.SubstituteEquations();
                            while(subResult != SubstitutionResult.NO_SUBSTITUTION_OCCURED)
                            {
                                subResult = this.SubstituteEquations();
                            }
                            if (subResult == SubstitutionResult.CONTRADICTION)
                            {
                                // TODO: handle contradiction
                                Environment.Exit(1);
                            }

                            done = false;
                            break;
                        }
                    }

                    if (!done)
                    {
                        break;
                    }
                }
            }
        }
    }
}
