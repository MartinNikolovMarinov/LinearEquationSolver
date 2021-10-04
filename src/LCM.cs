namespace LinearEquationSolver
{
    // Least Common Multiple
    public class LCM
    {
        public static long Calc(long a, long b)
        {
            return (a / GCD.Calc(a, b)) * b;
        }

        public static long Calc(long[] arr)
        {
            if (arr == null || arr.Length == 0) return 0;

            long ret = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                ret = LCM.Calc(ret, arr[i]);
            }

            return ret;
        }
    }
}
