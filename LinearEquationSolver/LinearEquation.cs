using System;
using System.Collections;
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
            foreach (var t in terms)
            {
                this.AddTerm(t);
            }
        }

        public IEnumerable<Term> GetTerms() => this.terms;
        public IEnumerable<long> GetNumberators() => this.terms.Select(x => x.Coefficient.Numberator);
        public IEnumerable<long> GetDenominators() => this.terms.Select(x => x.Coefficient.Denominator);
        public bool IsInvalid() => this.terms.Count == 1 && this.terms[0].IsConstant();
        public bool IsValid() => !this.IsInvalid();

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

        public override bool Equals(object obj)
        {
            if (obj is LinearEquation)
            {
                var other = ((LinearEquation)obj);
                if (this.terms.Count != other.terms.Count) 
                    return false;
                
                for (int i = 0; i < this.terms.Count; i++)
                {
                    if (this.terms[i] != other.terms[i]) 
                        return false;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LinearEquation a, LinearEquation b) => a.Equals(b);
        public static bool operator !=(LinearEquation a, LinearEquation b) => !a.Equals(b);

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
                // Terms containing only a constant. Can't build an equation but let's print somthing:
                return this.terms.First().Coefficient.Numberator.ToString();
            }

            bool reachedFirstOnLhs = false;
            // sb.Append(this.terms[0].ToString());
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

        public int CompareTo(LinearEquation other)
        {
            if (this.terms.Count == 0 && other.terms.Count == 0) return 0;
            else if (this.terms.Count == 0 && other.terms.Count != 0) return -1;
            else if (this.terms.Count != 0 && other.terms.Count == 0) return 1;

            var thisEnumerator = this.terms.GetEnumerator();
            var otherEnumberator = other.terms.GetEnumerator();
            int result = 0;
            bool firstHasMore = thisEnumerator.MoveNext();
            bool secondHasMore = otherEnumberator.MoveNext();
            while (firstHasMore && secondHasMore)
            {
                result = thisEnumerator.Current.CompareTo(otherEnumberator.Current);
                if (result != 0)
                {
                    result = result > 0 ? 1 : -1;
                    break;
                }

                firstHasMore = thisEnumerator.MoveNext();
                secondHasMore = otherEnumberator.MoveNext();
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
    }
}
