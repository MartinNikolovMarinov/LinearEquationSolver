namespace LinearEquationSolver
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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

        public IEnumerable<Term> GetTerms() => this.terms;
        public IEnumerable<long> GetNumberators() => this.terms.Select(x => x.Coefficient.Numberator);
        public IEnumerable<long> GetDenominators() => this.terms.Select(x => x.Coefficient.Denominator);

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

        public void MultEachTermBy(Fraction x)
        {
            for (int i = 0; i < this.terms.Count; i++)
            {
                var t = this.terms[i];
                t.Coefficient *= x;
            }
        }

        public void DivEachTermBy(Fraction x)
        {
            for (int i = 0; i < this.terms.Count; i++)
            {
                var t = this.terms[i];
                t.Coefficient /= x;
            }
        }

        public void Simplify()
        {
            if (this.terms.Count == 0) return;

            long numberatorGCD = GCD.Calc(this.GetNumberators().ToArray());
            long denominatorGCD = GCD.Calc(this.GetDenominators().ToArray());
#if DEBUG
            // Make sanity check in debug compilation mode - at this point, gcd should never be negative or 0!
            Debug.Assert(numberatorGCD > 0);
            Debug.Assert(denominatorGCD > 0);
#endif
            if (numberatorGCD > 1) this.DivEachTermBy((Fraction)numberatorGCD);
            if (denominatorGCD > 1) this.MultEachTermBy((Fraction)denominatorGCD);
        }

        public override string ToString()
        {
#if DEBUG
            int _dcount = 0;
#endif

            StringBuilder sb = new StringBuilder();
            Fraction constant = (Fraction)0;

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
                    constant = t.Coefficient;
#if DEBUG
                    // Make sanity check in debug compilation mode - Both these cases should have been handled by this point!
                    Debug.Assert(_dcount == this.terms.Count - 2);
#endif
                    // it should be the last element so we break:
                    break;
                }

                if (t.IsPositive())
                {
                    sb.Append($" + {t}");
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

            // Move the constant to the right hand side:
            sb.Append($" = {-constant}");

            return sb.ToString();
        }
    }
}
