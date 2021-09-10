namespace LinearEquationSolver
{
    // Least Common Multiple
    public class LCM
    {
        public static long Calculate(long a, long b)
        {
            return (a / GCD.Calc(a, b)) * b;
        }
    }
}
