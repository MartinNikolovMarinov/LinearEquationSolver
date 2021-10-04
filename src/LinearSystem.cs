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
            NO_SUBSTITUTION_OCCURRED,
            SUBSTITUTION_OCCURRED,
            CONTRADICTION,
        }

        public SubstitutionResult SubstituteEquations()
        {
            SubstitutionResult result = SubstitutionResult.NO_SUBSTITUTION_OCCURRED;
            for (int i = 0; i < this.equations.Count; i++)
            {
                LinearEquation curr = this.equations[i];
                if (curr.IsInvalid())
                {
                    // Contradiction found
                    result = SubstitutionResult.CONTRADICTION;
                    goto DONE;
                }

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
                                result = SubstitutionResult.SUBSTITUTION_OCCURRED;
                            }
                        }
                    }
                }
            }

        DONE:
            return result;
        }

        public class ReductionResult
        {
            // Which row did it use for the reduction:
            public int SrcReduceRow;
            // In which row did the reduction occur:
            public int DestReduceRow;
            // What was the row reduced by:
            public Fraction ReductionCoefficient;

            public override string ToString()
            {
                return $"{this.ReductionCoefficient} * p_{{{this.SrcReduceRow}}} + p_{{{this.DestReduceRow}}}";
            }
        }

        // ReduceEquation uses Gausses method to reduce the system in exactly one row.
        public ReductionResult ReduceEquation()
        {
            ReductionResult result = new ReductionResult
            {
                SrcReduceRow = -1,
                DestReduceRow = -1,
                ReductionCoefficient = (Fraction)0,
            };

            for (int i = 0; i < this.equations.Count; i++)
            {
                LinearEquation curr = this.equations[i];
                Term currLeadingTerm = curr.GetLeadingTerm();
                if (curr.HasSolution()) continue;

                for (int j = i + 1; j < this.equations.Count; j++)
                {
                    LinearEquation next = this.equations[j];
                    Term nextLeadingTerm = next.GetLeadingTerm();
                    if (next.HasSolution()) continue;

                    if (currLeadingTerm.Variable == nextLeadingTerm.Variable)
                    {
                        Fraction x = ((Fraction)(-1)) * (currLeadingTerm.Coefficient / nextLeadingTerm.Coefficient);
                        result.SrcReduceRow = i;
                        result.DestReduceRow = j;
                        result.ReductionCoefficient = Fraction.One / x;
                        LinearEquation reductionEquation = new LinearEquation(curr);
                        reductionEquation.DivEachTermBy(x);
                        foreach (Term t in reductionEquation.GetTerms())
                        {
                            next.AddTerm(t);
                        }
                        next.Simplify();

                        goto DONE;
                    }
                }
            }

        DONE:
            if (result.SrcReduceRow < 0) 
                return null;
            return result;
        }

        // Tries to solve the linear equation. Returns false if it contains a contradiction.
        public bool DirectSolve()
        {
            ReductionResult reductionResult = null;
            bool hasContradiction = false;
            do
            {
                reductionResult = this.ReduceEquation();
                SubstitutionResult subResult = this.SubstituteEquations();
                while (subResult == SubstitutionResult.SUBSTITUTION_OCCURRED)
                {
                    subResult = this.SubstituteEquations();
                }
                hasContradiction = (subResult == SubstitutionResult.CONTRADICTION);
            }
            while (reductionResult != null && !hasContradiction);

            return hasContradiction;
        }
    }
}
