namespace Enjoy.Application.Abstractions.Math;

public interface IMathService
{
    long GreatestCommonDivisor(long a, long b);

    long LeastCommonMultiple(long a, long b);

    long LeastCommonMultiple(IEnumerable<long> numbers);
}
