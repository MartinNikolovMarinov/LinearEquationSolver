namespace LinearEquationSolver
{
    using System;
    using System.Runtime.CompilerServices;

    public class Fraction : IComparable<Fraction>
    {
        public static readonly Fraction Zero = new Fraction(0, 0);
        public static readonly Fraction One = new Fraction(1, 1);

        public static Fraction Simplify(Fraction _frac)
        {
            // Handle zero cases:
            if (_frac.Numberator == 0) return new Fraction(Fraction.Zero);
            
            var ret = new Fraction(_frac);
            long gcd = GCD.Calc(ret.Numberator, ret.Denominator);
            ret.Numberator /= gcd;
            ret.Denominator /= gcd;

            // Handle both negatative case:
            if (ret.numberator < 0 && ret.denominator < 0)
            {
                ret.numberator = -ret.numberator;
                ret.denominator = -ret.denominator;
            }

            return ret;
        }

        public static Fraction Add(Fraction _f1, Fraction _f2)
        {
            // Handle zero cases:
            if (_f1.Denominator == 0) return _f2;
            if (_f2.Denominator == 0) return _f1;

            var ret = new Fraction(_f1);
            ret.Numberator = ret.Numberator * _f2.Denominator + ret.Denominator * _f2.Numberator;
            ret.Denominator = ret.Denominator * _f2.Denominator;
            return Fraction.Simplify(ret);
        }

        public static Fraction Sub(Fraction _f1, Fraction _f2)
        {
            // Handle zero cases:
            if (_f1.Denominator == 0) return -_f2;
            if (_f2.Denominator == 0) return _f1;

            var ret = new Fraction(_f1);
            ret.Numberator = ret.Numberator * _f2.Denominator - ret.Denominator * _f2.Numberator;
            ret.Denominator = ret.Denominator * _f2.Denominator;
            return Fraction.Simplify(ret);
        }

        public static Fraction Mult(Fraction _f1, Fraction _f2)
        {
            var ret = new Fraction(_f1);
            ret.Numberator *= _f2.Numberator;
            ret.Denominator *= _f2.Denominator;
            return ret;
        }

        public static Fraction Div(Fraction _f1, Fraction _f2)
        {
            var ret = new Fraction(_f1);
            ret.Denominator *= _f2.Numberator;
            ret.Numberator *= _f2.Denominator;
            return ret;
        }

        private long numberator;
        private long denominator;

        public long Numberator
        {
            get { return this.numberator; }
            set
            {
                this.CheckValid(value, this.denominator);
                this.numberator = value;
                this.FixSigns();
                this.FixZeroNumberator();
            }
        }

        public long Denominator
        {
            get { return this.denominator; }
            set
            {
                this.CheckValid(this.numberator, value);
                this.denominator = value;
                this.FixSigns();
                this.FixZeroNumberator();
            }
        }

        public Fraction() : this(0, 0) { }
        public Fraction(Fraction _f) : this(_f.Numberator, _f.Denominator) { }
        public Fraction(long _numberator, long _denominator)
        {
            this.CheckValid(_numberator, _denominator);
            this.numberator = _numberator;
            this.denominator = _denominator;
            this.FixSigns();
            this.FixZeroNumberator();
        }

        public static explicit operator Fraction(sbyte _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(byte _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(short _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(int _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(long _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(ushort _a) => new Fraction(_a, 1);
        public static explicit operator Fraction(uint _a) => new Fraction(_a, 1);
        //public static explicit operator Fraction(ulong _a) => new Fraction(_a, 1); Note that this can overflow the numberator

        public Fraction Add(Fraction _other) => Fraction.Add(this, _other);

        public static Fraction operator +(Fraction _a) => new Fraction(_a.Numberator, _a.Denominator);

        public static Fraction operator +(Fraction _a, Fraction _b) => _a.Add(_b);

        public Fraction Sub(Fraction _other) => Fraction.Sub(this, _other);

        public static Fraction operator -(Fraction _a) => new Fraction(-_a.Numberator, _a.Denominator);

        public static Fraction operator -(Fraction _a, Fraction _b) => _a.Sub(_b);

        public Fraction Mult(Fraction _other) => Fraction.Mult(this, _other);

        public static Fraction operator *(Fraction _a, Fraction _b) => _a.Mult(_b);

        public Fraction Div(Fraction _other) => Fraction.Div(this, _other);

        public static Fraction operator /(Fraction _a, Fraction _b) => _a.Div(_b);

        public override bool Equals(object _obj)
        {
            if (_obj is Fraction)
            {
                var other = Fraction.Simplify((Fraction)_obj);
                var simpleThis = Fraction.Simplify(this);
                return (other.Denominator == simpleThis.Denominator && other.Numberator == simpleThis.Numberator);
            }
            return false;
        }

        public static bool operator ==(Fraction _a, Fraction _b) => _a.Equals(_b);

        public static bool operator !=(Fraction _a, Fraction _b) => !_a.Equals(_b);

        public override int GetHashCode()
        {
            return HashCode.Combine(Numberator, Denominator);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Numberator, this.Denominator);
        }

        public int CompareTo(Fraction _other)
        {
            long lcm = LCM.Calculate(_other.Denominator, this.Denominator);
            long diff = this.Numberator * (lcm / this.Denominator) - _other.Numberator * (lcm / _other.Denominator);
            if (diff > 0) return 1;
            if (diff < 0) return -1;
            else return 0;
        }

        public static bool operator >(Fraction _a, Fraction _b) => _a.CompareTo(_b) > 0;

        public static bool operator >=(Fraction _a, Fraction _b) => _a.CompareTo(_b) >= 0;

        public static bool operator <(Fraction _a, Fraction _b) => _a.CompareTo(_b) < 0;

        public static bool operator <=(Fraction _a, Fraction _b) => _a.CompareTo(_b) <= 0;

        private void CheckValid(long _numberator, long _denominator)
        {
            if (_numberator != 0 && _denominator == 0) throw new InvalidOperationException("Trying to divide by zero");
        }

        private void FixSigns()
        {
            if ((this.numberator < 0 && this.denominator < 0) ||
                this.numberator > 0 && this.denominator < 0)
            {
                this.numberator = -this.numberator;
                this.denominator = -this.denominator;
            }
        }

        private void FixZeroNumberator()
        {
            if (this.numberator == 0 && this.denominator != 0) this.denominator = 0;
        }
    }
}
