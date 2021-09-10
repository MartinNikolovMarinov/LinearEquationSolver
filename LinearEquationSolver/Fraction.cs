namespace LinearEquationSolver
{
    using System;
    using System.Diagnostics;

    public struct Fraction : IComparable<Fraction>
    {
        public static readonly Fraction Zero = new Fraction(0, 0);
        public static readonly Fraction One = new Fraction(1, 1);

        public static Fraction Add(Fraction lhs, Fraction rhs)
        {
            // Additions with 0 are special:
            if (lhs.denominator == 0) return new Fraction(rhs);
            if (rhs.denominator == 0) return new Fraction(lhs);

            long numberator = lhs.numberator * rhs.denominator + rhs.numberator * lhs.denominator;
            long denominator = lhs.denominator * rhs.denominator;
            var ret = new Fraction(numberator, denominator);
            return ret;
        }

        public static Fraction Sub(Fraction lhs, Fraction rhs)
        {
            return Fraction.Add(lhs, -rhs);
        }

        public static Fraction Mult(Fraction lhs, Fraction rhs)
        {
            long numberator = lhs.numberator * rhs.numberator;
            long denominator = lhs.denominator * rhs.denominator;
            var ret = new Fraction(numberator, denominator);
            return ret;
        }

        public static Fraction Div(Fraction lhs, Fraction rhs)
        {
            long numberator = lhs.numberator * rhs.denominator;
            long denominator = lhs.denominator * rhs.numberator;
            var ret = new Fraction(numberator, denominator);
            return ret;
        }

        private long numberator;
        private long denominator;

        public long Numberator => this.numberator;
        public long Denominator => this.denominator;

        public Fraction(long numberator, long denominator)
        {
            this.numberator = numberator;
            this.denominator = denominator;
            this.Simplify();
        }
        public Fraction(Fraction f)
        {
            this.numberator = f.numberator;
            this.denominator = f.denominator;
            // No need to simplify on copy.
        }

        public static explicit operator Fraction(sbyte x) => new Fraction(x, 1);
        public static explicit operator Fraction(byte x) => new Fraction(x, 1);
        public static explicit operator Fraction(short x) => new Fraction(x, 1);
        public static explicit operator Fraction(int x) => new Fraction(x, 1);
        public static explicit operator Fraction(long x) => new Fraction(x, 1);
        public static explicit operator Fraction(ushort x) => new Fraction(x, 1);
        public static explicit operator Fraction(uint x) => new Fraction(x, 1);
        // public static explicit operator Fraction(ulong x) => new Fraction(x, 1); Note that this can overflow the numberator

        public Fraction Add(Fraction other) => Fraction.Add(this, other);
        public static Fraction operator +(Fraction x) => new Fraction(x.numberator, x.denominator);
        public static Fraction operator +(Fraction a, Fraction b) => a.Add(b);
        public Fraction Sub(Fraction other) => Fraction.Sub(this, other);
        public static Fraction operator -(Fraction x) => new Fraction(-x.numberator, x.denominator);
        public static Fraction operator -(Fraction a, Fraction b) => a.Sub(b);
        public Fraction Mult(Fraction other) => Fraction.Mult(this, other);
        public static Fraction operator *(Fraction a, Fraction b) => a.Mult(b);
        public Fraction Div(Fraction other) => Fraction.Div(this, other);
        public static Fraction operator /(Fraction a, Fraction b) => a.Div(b);

        public override bool Equals(object obj)
        {
            if (obj is Fraction)
            {
                var objFrac = (Fraction)(obj);
                return (this.denominator == objFrac.denominator && this.numberator == objFrac.numberator);
            }
            return false;
        }

        public static bool operator ==(Fraction a, Fraction b) => a.Equals(b);
        public static bool operator !=(Fraction a, Fraction b) => !a.Equals(b);

        public override int GetHashCode()
        {
            return HashCode.Combine(this.numberator, this.denominator);
        }

        public int CompareTo(Fraction other)
        {
            long lcm = LCM.Calculate(other.numberator, this.denominator);
            long diff = this.numberator * (lcm / this.denominator) - other.numberator * (lcm / other.denominator);
            if (diff > 0) return 1;
            if (diff < 0) return -1;
            else return 0;
        }

        public static bool operator >(Fraction a, Fraction b) => a.CompareTo(b) > 0;
        public static bool operator >=(Fraction a, Fraction b) => a.CompareTo(b) >= 0;
        public static bool operator <(Fraction a, Fraction b) => a.CompareTo(b) < 0;
        public static bool operator <=(Fraction a, Fraction b) => a.CompareTo(b) <= 0;

        public override string ToString()
        {
            // TODO: fix this nonsense:
            if (this.numberator == 0) return "0";
            if (this.denominator == 1) return this.numberator.ToString();
            return string.Format("{0}/{1}", this.numberator, this.denominator);
        }

        public bool IsPositive() => this.numberator > 0;
        public bool IsInteger() => this.denominator == 1;

        private void CheckDivisionByZero()
        {
            if (this.numberator != 0 && this.denominator == 0)
            {
                throw new InvalidOperationException("Divide by zero");
            }
        }

        private void Simplify()
        {
            this.CheckDivisionByZero();

            // Handle zero case:
            if (this.numberator == 0)
            {
                this = Fraction.Zero;
                return;
            }

#if DEBUG
            // Make sanity check in debug compilation mode - Both these cases should have been handled by this point!
            Debug.Assert(this.numberator != 0 && this.denominator != 0);
#endif

            long gcd = GCD.Calc(this.numberator, this.denominator);
            this.numberator /= gcd;
            this.denominator /= gcd;

            // Handle equation sign:
            if ((this.numberator < 0 && this.denominator < 0) ||
                this.numberator > 0 && this.denominator < 0)
            {
                this.numberator = -this.numberator;
                this.denominator = -this.denominator;
            }
#if DEBUG
            // Make sanity check in debug compilation mode - Denominator should never be negative!
            bool validCase1 = this.numberator > 0 && this.denominator > 0;
            bool validCase2 = this.numberator < 0 && this.denominator > 0;
            Debug.Assert(validCase1 || validCase2);
#endif
        }
    }
}
