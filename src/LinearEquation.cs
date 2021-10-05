using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LinearEquationSolver
{
    public class LinearEquation : IComparable<LinearEquation>
    {
        private List<Term> terms;

        public LinearEquation(params Term[] terms)
        {
            this.terms = new List<Term>(terms.Length);
            foreach (Term t in terms)
            {
                this.AddTerm(t);
            }
        }
        public LinearEquation(LinearEquation other)
        {
            this.terms = new List<Term>(other.terms.Count);
            foreach (Term t in other.terms)
            {
                this.AddTerm(t);
            }
        }

        public IEnumerable<Term> GetTerms() => this.terms;
        public Term GetLeadingTerm() => this.terms.FirstOrDefault();
        public IEnumerable<long> GetNumberators() => this.terms.Select(x => x.Coefficient.Numberator);
        public IEnumerable<long> GetDenominators() => this.terms.Select(x => x.Coefficient.Denominator);
        public bool IsInvalid() => this.terms.Count == 1 && this.terms[0].IsConstant();
        public bool IsValid() => !this.IsInvalid();

        public void AddTerm(Term term)
        {
            Term termCopy = new Term(term);
            if (termCopy.Coefficient == Fraction.Zero) return;

            int iOfTerm = this.terms.FindIndex((x) => x.Variable.Trim() == termCopy.Variable.Trim());
            if (iOfTerm >= 0)
            {
                // If the term exists add the coefficients:
                Term exitingTerm = this.terms[iOfTerm];
                exitingTerm.Coefficient += termCopy.Coefficient;
                if (exitingTerm.Coefficient == (Fraction)0)
                {
                    // Remove terms that evaluate to 0:
                    this.terms.RemoveAt(iOfTerm);
                }
            }
            else
            {
                this.terms.Add(termCopy);
                this.terms.Sort((a, b) => b.CompareTo(a));
            }
        }

        public void MultEachTermBy(Fraction x)
        {
            for (int i = 0; i < this.terms.Count; i++)
            {
                Term t = this.terms[i];
                t.Coefficient *= x;
            }
        }

        public void DivEachTermBy(Fraction x)
        {
            for (int i = 0; i < this.terms.Count; i++)
            {
                Term t = this.terms[i];
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

        public bool Substitute(string variable, Fraction fraction)
        {
            int iOfTerm = this.terms.FindIndex((x) => x.Variable.Trim() == variable.Trim());
            if (iOfTerm >= 0)
            {
                Term exitingTerm = this.terms[iOfTerm];
                this.terms.RemoveAt(iOfTerm);
                exitingTerm.Variable = "";
                //exitingTerm.Coefficient = exitingTerm.Coefficient.Abs();
                exitingTerm.Coefficient *= fraction;
                this.AddTerm(exitingTerm);
                this.Simplify();
                return true;
            }

            return false;
        }

        public class Solution
        {
            public string Variable { get; set; }
            public Fraction Value { get; set; }

            public Solution() { }
            public Solution(string variable, Fraction value, bool hasNoSolution = false)
            {
                this.Variable = variable;
                this.Value = value;
            }
        }

        public Solution GetSolution()
        {
            if (this.IsInvalid()) 
                return null;

            List<Term> terms = this.terms;
            int count = terms.Count();
            if (count == 0)
            {
                Solution ret = new Solution("0", (Fraction)0);
                return ret;
            }
            else if (count == 1)
            {
                if (!terms[0].IsConstant())
                {
                    // In this case the variable is equal to 0. Example 1/2x=0
                    Solution ret = new Solution(terms[0].Variable, (Fraction)0);
                    return ret;
                }
                else
                {
                    // Equation is not valid in this case. Example 0=5/2
                    throw new Exception("Internal Implementation Error. This case should have been handled previously. This is a bug.");
                }
            }
            else if (count == 2)
            {
                if (terms[0].IsConstant())
                    throw new Exception("Internal Implementation Error. This is a bug.");

                if (terms[1].IsConstant())
                {
                    // Example for this case is:
                    // 1/2x + 2 = 0
                    // 1/2x = -2
                    // x = (-2) / (1/2)
                    Solution ret = new Solution(terms[0].Variable, (-terms[1].Coefficient) / terms[0].Coefficient);
                    return ret;
                }

                // No constants -> parameterized solution.
                return null;
            }

            // More than 2 terms means that the solution is parameterized.
            return null;
        }

        public bool HasSolution() => this.GetSolution() != null;

        public override bool Equals(object obj)
        {
            if (obj is LinearEquation)
            {
                LinearEquation other = (LinearEquation)obj;
                if (this.terms.Count != other.terms.Count) return false;

                for (int i = 0; i < this.terms.Count; i++)
                {
                    if (this.terms[i] != other.terms[i]) return false;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(LinearEquation a, LinearEquation b) => a.Equals(b);
        public static bool operator !=(LinearEquation a, LinearEquation b) => !a.Equals(b);

        public int CompareTo(LinearEquation other)
        {
            if (this.terms.Count == 0 && other.terms.Count == 0) return 0;
            else if (this.terms.Count == 0 && other.terms.Count != 0) return -1;
            else if (this.terms.Count != 0 && other.terms.Count == 0) return 1;

            List<Term>.Enumerator thisEnumerator = this.terms.GetEnumerator();
            List<Term>.Enumerator otherEnumerator = other.terms.GetEnumerator();
            int result = 0;
            bool firstHasMore = thisEnumerator.MoveNext();
            bool secondHasMore = otherEnumerator.MoveNext();
            while (firstHasMore && secondHasMore)
            {
                result = thisEnumerator.Current.CompareTo(otherEnumerator.Current);
                if (result != 0)
                {
                    result = result > 0 ? 1 : -1;
                    break;
                }

                firstHasMore = thisEnumerator.MoveNext();
                secondHasMore = otherEnumerator.MoveNext();
            }

            if (!firstHasMore && !secondHasMore) result = 0;
            else if (!firstHasMore) result = 1;
            else if (!secondHasMore) result = -1;

            return result;
        }

        public static bool operator >(LinearEquation a, LinearEquation b) => a.CompareTo(b) > 0;
        public static bool operator >=(LinearEquation a, LinearEquation b) => a.CompareTo(b) >= 0;
        public static bool operator <(LinearEquation a, LinearEquation b) => a.CompareTo(b) < 0;
        public static bool operator <=(LinearEquation a, LinearEquation b) => a.CompareTo(b) <= 0;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Fraction constant = (Fraction)0;

            if (this.terms.Count == 0)
            {
                // No terms. Can't build an equation.
                return "";
            }
            else if (this.IsInvalid())
            {
                // Terms containing only a constant. Can't build an equation but let's print something:
                return this.terms.First().Coefficient.Abs().ToString() + " = 0";
            }

            bool reachedFirstOnLhs = false;
            for (int i = 0; i < this.terms.Count; i++)
            {
                Term t = this.terms[i];
                if (t.IsConstant())
                {
                    // found the constant
                    constant = t.Coefficient;
                }
                else
                {
                    if (!reachedFirstOnLhs)
                    {
                        sb.Append(t);
                        reachedFirstOnLhs = true;
                    }
                    else if (t.IsPositive())
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
                }
            }

            // Move the constant to the right hand side:
            sb.Append($" = {-constant}");
            return sb.ToString();
        }
    }
}
