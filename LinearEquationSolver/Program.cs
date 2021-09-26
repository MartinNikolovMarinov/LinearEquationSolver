using LinearEquationSolver.Parsers;

namespace LinearEquationSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            ILinearEquationParser lep = new BasicLinearEquationParser();
            var ls = new LinearSystem();
            ls.AddEquation(lep.Parse("x + y = 0"));
            ls.AddEquation(lep.Parse("2x - y + 3z = 3"));
            ls.AddEquation(lep.Parse("x - 2y - z = 3"));
            ls.Solve();
            System.Console.WriteLine(string.Join("\n", ls.Equations));

            //ILinearEquationParser lep = new BasicLinearEquationParser();
            //var ls = new LinearSystem();
            //ls.AddEquation(lep.Parse("x = 1"));
            //ls.AddEquation(lep.Parse("-5y + 2z = -5"));
            //ls.AddEquation(lep.Parse("y + 2x = 11"));
            //bool subResult = ls.SubstituteEquations();
            //System.Console.WriteLine(subResult);
            //System.Console.WriteLine(string.Join("\n", ls.GetEquations));
        }
    }
}
