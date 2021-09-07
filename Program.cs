using System;

namespace LinearEquationSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Fraction f1 = new Fraction(1, 2);
            Fraction f2 = new Fraction(5, 6);
            Fraction f3 = new Fraction(2, 8);
            Fraction f4 = new Fraction(18, 3);
            Fraction f5 = new Fraction(-21, 7);
            Fraction f6 = new Fraction(-12, -6);
            Fraction f7 = new Fraction(99, -9);

            Console.WriteLine(Fraction.Simplify(f1));
            Console.WriteLine(Fraction.Simplify(f2));
            Console.WriteLine(Fraction.Simplify(f3));
            Console.WriteLine(Fraction.Simplify(f4));
            Console.WriteLine(Fraction.Simplify(f5));
            Console.WriteLine(Fraction.Simplify(f6));
            Console.WriteLine(Fraction.Simplify(f7));
        }
    }
}
