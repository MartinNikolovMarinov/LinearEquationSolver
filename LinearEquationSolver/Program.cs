using LinearEquationSolver.Parsers;

namespace LinearEquationSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            ILinearEquationParser lep = new BasicLinearEquation();
            LinearEquation eq1 = lep.Parse("1a + x/2 = 1");
            LinearEquation eq2 = lep.Parse("c + a/2 = 1");
            LinearEquation eq3 = lep.Parse("c + 2/a = 6"); // this parses completely wrong!
            var ls = new LinearSystem(eq1, eq2);
            ls.AddEquation(eq3);
            ls.SolveNextStep();
        }
    }
}
