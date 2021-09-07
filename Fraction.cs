namespace LinearEquationSolver
{
    using System;

    public class Fraction : IComparable<Fraction>
    {
        public static void IsValid(Fraction f)
        {
            if (f.Numberator != 0 && f.Denominator == 0) throw new InvalidOperationException("Trying to divide by zero");
        }

        public static Fraction Simplify(Fraction frac)
        {
            var ret = new Fraction(frac);
            long gcd = GCD.Calc(ret.Numberator, ret.Denominator);
            ret.Numberator /= gcd;
            ret.Denominator /= gcd;
            return ret;
        }

        public long Numberator { get; set; }

        public long Denominator { get; set; }

        public Fraction() { }
        public Fraction(Fraction f) : this(f.Numberator, f.Denominator) {}
        public Fraction(long _numberator, long _denominator) 
        {
            this.Numberator = _numberator;
            this.Denominator = _denominator;
            Fraction.IsValid(this);
        }

        public Fraction Add(Fraction _other)
        {
            var ret = new Fraction(this.Numberator, this.Denominator);
            ret.Numberator += _other.Numberator;
            ret.Denominator += _other.Denominator;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Add(long _other)
        {
            var ret = new Fraction(this);
            ret.Numberator += _other;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Sub(Fraction _other)
        {
            var ret = new Fraction(this);
            ret.Numberator -= _other.Numberator;
            ret.Denominator -= _other.Denominator;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Sub(long _other)
        {
            var ret = new Fraction(this);
            ret.Numberator -= _other;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Mult(Fraction _other)
        {
            var ret = new Fraction(this);
            ret.Numberator *= _other.Numberator;
            ret.Denominator *= _other.Denominator;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Mult(long _other)
        {
            var ret = new Fraction(this);
            ret.Numberator *= _other;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Div(Fraction _other)
        {
            var ret = new Fraction(this);
            ret.Numberator /= _other.Numberator;
            ret.Denominator /= _other.Denominator;
            Fraction.IsValid(ret);
            return ret;
        }

        public Fraction Div(long _other)
        {
            var ret = new Fraction(this);
            ret.Numberator /= _other;
            Fraction.IsValid(ret);
            return ret;
        }

        public override bool Equals(object obj)
        {
            if (obj is Fraction)
            {
                var other = (Fraction)obj;
                bool areEqual = other.Numberator == this.Numberator && other.Denominator == this.Denominator;
                return areEqual;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Numberator, Denominator);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Numberator, this.Denominator);
        }

        public int CompareTo(Fraction other)
        {
            long gcd = GCD.Calc(other.Denominator, this.Denominator);
            long thisNumberator = this.Numberator * gcd;
            long otherNumberator = other.Numberator * gcd;

            if (thisNumberator > otherNumberator) return 1;
            else return -1;
        }
    }
}
