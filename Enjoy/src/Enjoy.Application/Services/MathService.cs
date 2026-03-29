using Enjoy.Application.Abstractions.Math;

namespace Enjoy.Application.Services;

internal sealed class MathService : IMathService
{
    public long GreatestCommonDivisor(long a, long b)
    {
        a = System.Math.Abs(a);
        b = System.Math.Abs(b);

        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public long LeastCommonMultiple(long a, long b)
    {
        if (a == 0 || b == 0)
            return 0;

        return System.Math.Abs(a / GreatestCommonDivisor(a, b) * b);
    }

    public long LeastCommonMultiple(IEnumerable<long> numbers)
    {
        return numbers.Aggregate(1L, LeastCommonMultiple);
    }
}
