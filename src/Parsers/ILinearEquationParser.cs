namespace LinearEquationSolver.Parsers
{
    public interface ILinearEquationParser
    {
        LinearEquation Parse(string rawInput);
    }
}
