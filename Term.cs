namespace LinearEquationSolver
{
    using System;

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
                if (String.IsNullOrWhiteSpace(value)) value = "";
                this.variable = value;
            }
        }

        public Term(Fraction coef, string variable)
        {
            this.Coefficient = coef;
            this.Variable = variable;
        }

        public int CompareTo(Term othert)
        {
            if (this.Variable != othert.Variable)
            {
                return this.Variable.CompareTo(othert.Variable);
            }
            // If they have equal variables compare the coefficient:
            return this.Coefficient.CompareTo(othert.Coefficient);
        }

        public static bool operator >(Term a, Term b) => a.CompareTo(b) > 0;
        public static bool operator >=(Term a, Term b) => a.CompareTo(b) >= 0;
        public static bool operator <(Term a, Term b) => a.CompareTo(b) < 0;
        public static bool operator <=(Term a, Term b) => a.CompareTo(b) <= 0;

        public bool IsPositive() => this.coefficient.IsPositive();

        public override bool Equals(object obj)
        {
            if (obj is Term)
            {
                var termObj = ((Term)obj);
                return this.Variable == termObj.Variable && this.Coefficient == termObj.Coefficient;
            }

            return false;
        }

        public static bool operator ==(Term a, Term b) => a.Equals(b);
        public static bool operator !=(Term a, Term b) => !a.Equals(b);

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Coefficient, this.Variable);
        }

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(this.Variable)) return this.Coefficient.ToString(); // Silly special case.
            return string.Format("{0} {1}", this.Coefficient, this.Variable);
        }

        public bool IsConstant() => this.coefficient.IsInteger() && String.IsNullOrWhiteSpace(this.variable);
    }
}
