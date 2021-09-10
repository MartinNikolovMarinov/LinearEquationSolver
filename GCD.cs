namespace LinearEquationSolver
{
    // Greatest Common Divisor
    public static class GCD
    {
        public static long Calc(long a, long b)
        {
            a = (a > 0) ? a : -a;
            b = (b > 0) ? b : -b;

            while (a != 0 && b != 0)
            {
                if (a > b) a %= b;
                else b %= a;
            }

            return a | b;
        }
    }
}
