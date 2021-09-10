using System;

namespace LinearEquationSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Fraction f1 = new Fraction(4, 8);
            f1 += (Fraction)1;

            Console.WriteLine(f1);
        }
    }
}
