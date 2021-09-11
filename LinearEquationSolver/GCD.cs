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

        public static long Calc(long[] arr)
        {
            if (arr == null || arr.Length == 0) return 0;

            long ret = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                ret = GCD.Calc(ret, arr[i]);
                if (ret == 1) break;
            }

            return ret;
        }
    }
}
