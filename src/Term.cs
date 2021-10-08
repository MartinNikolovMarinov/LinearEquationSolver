using System;

namespace LinearEquationSolver
{
    public class Term : IComparable<Term>
    {
        private Fraction coefficient;
        private string variable;

        public Fraction Coefficient
        {
            get { return this.coefficient; }
            set
            {
                if (value == null) value = Fraction.Zero;
                this.coefficient = value;
            }
        }

        public string Variable
        {
            get { return this.variable; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = "";
                this.variable = value;
            }
        }

        public Term(Fraction coefficient, string variable)
        {
            this.Coefficient = coefficient;
            this.Variable = variable;
        }
        public Term(Term other) : this(other.coefficient, other.variable) { }

        public static int CompareTo(Term a, Term b)
        {
            if (a == null && b == null) 
                return 0;
            if (a != null && b == null)
                return 1;
            if (a == null && b != null)
                return -1;

            if (a.Variable != b.Variable) 
                return a.Variable.CompareTo(b.Variable);
            
            // If they have equal variables compare the coefficient:
            return a.Coefficient.CompareTo(b.Coefficient);
        }

        public int CompareTo(Term other) => Term.CompareTo(this, other);

        public static bool operator >(Term a, Term b) => Term.CompareTo(a, b) > 0;
        public static bool operator >=(Term a, Term b) => Term.CompareTo(a, b) >= 0;
        public static bool operator <(Term a, Term b) => Term.CompareTo(a, b) < 0;
        public static bool operator <=(Term a, Term b) => Term.CompareTo(a, b) <= 0;

        public bool IsPositive() => this.coefficient.IsPositive();

        public override bool Equals(object obj)
        {
            if (obj is Term)
            {
                Term termObj = ((Term)obj);
                return this.Variable == termObj.Variable && this.Coefficient == termObj.Coefficient;
            }

            return false;
        }

        public static bool operator ==(Term a, Term b) {
            if (a is null && b is null) return true;
            else if (a is null || b is null) return false;
            else return a.Equals(b);
        }
        public static bool operator !=(Term a, Term b)
        {
            if (a is null && b is null) return false;
            else if (a is null || b is null) return true;
            else return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Coefficient, this.Variable);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Variable)) return this.Coefficient.ToString();
            if (this.Coefficient == Fraction.One) return this.Variable.ToString();
            if (this.Coefficient == -Fraction.One) return $"-{this.Variable}";
            return $"{this.Coefficient} {this.Variable}";
        }

        public bool IsConstant() => String.IsNullOrWhiteSpace(this.variable);
    }
}
