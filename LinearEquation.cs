namespace LinearEquationSolver
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public class LinearEquation
    {
        private List<Term> terms;

        public LinearEquation(params Term[] terms)
        {
            this.terms = new List<Term>(terms.Length);
            foreach (var t in terms)
            {
                this.AddTerm(t);
            }
        }

        public void AddTerm(Term t)
        {
            if (t.Coefficient == Fraction.Zero) return;

            int iOfTerm = this.terms.FindIndex((x) => x.Variable.Trim() == t.Variable.Trim());
            if (iOfTerm >= 0)
            {
                // If the term exists add the coefficients:
                var exitingTerm = this.terms[iOfTerm];
                exitingTerm.Coefficient += t.Coefficient;
                if (exitingTerm.Coefficient == (Fraction)0)
                {
                    // Remove terms that evaluate to 0:
                    this.terms.RemoveAt(iOfTerm);
                }
            } 
            else
            {
                this.terms.Add(t);
                this.terms.Sort((a, b) => b.CompareTo(a));
            }
        }

        public IEnumerable<Term> GetTerms() => this.terms;

        public override string ToString()
        {
#if DEBUG
            int _dcount = 0;
#endif

            StringBuilder sb = new StringBuilder();
            long constant = 0;

            if (this.terms.Count == 0)
            {
                // No terms. Can't build an equation.
                return "";
            }
            else if (this.terms.Count == 1 && this.terms[0].IsConstant())
            {
                // Terms contain only a constant. Can't build an equation.
                return "";
            }

            sb.Append(this.terms[0].ToString());
            for (int i = 1; i < this.terms.Count; i++)
            {
                Term t = this.terms[i];
                if (t.IsConstant())
                {
                    // found the constant
                    constant = t.Coefficient.Numberator;
#if DEBUG
                    // Make sanity check in debug compilation mode - Both these cases should have been handled by this point!
                    Debug.Assert(_dcount == this.terms.Count - 2);
#endif
                    // it should be the last element so we break:
                    break;
                }

                if (t.IsPositive())
                {
                    sb.Append(" + ");
                    sb.Append(t.ToString());
                }
                else
                {
                    sb.Append(" - ");
                    // The fraction prints negative signs but in this case it's not needed:
                    string negFrac = t.ToString();
                    int iOfSign = negFrac.IndexOf("-") + 1;
                    sb.Append(negFrac, negFrac.IndexOf("-") + 1, negFrac.Length - iOfSign);
                }

#if DEBUG
                _dcount++;
#endif
            }

            sb.Append(" = ");
            sb.Append(constant);

            return sb.ToString();
        }
    }
}
