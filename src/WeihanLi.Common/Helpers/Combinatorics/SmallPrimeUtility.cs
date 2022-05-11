using System.Collections;
using System.Numerics;

namespace WeihanLi.Common.Helpers.Combinatorics;

/// <summary>
/// Utility class that maintains a small table of prime numbers and provides
/// simple implementations of Prime Factorization algorithms.
/// This is a quick and dirty utility class to support calculations of permutation
/// sets with indexes under 2^31.
/// The prime table contains all primes up to Sqrt(2^31) which are all of the primes
/// requires to factorize any Int32 positive integer.
/// </summary>
internal static class SmallPrimeUtility
{
    /// <summary>
    /// Performs a prime factorization of a given integer using the table of primes in PrimeTable.
    /// Since this will only factor Int32 sized integers, a simple list of factors is returned instead
    /// of the more scalable, but more difficult to consume, list of primes and associated exponents.
    /// </summary>
    /// <param name="i">The number to factorize, must be positive.</param>
    /// <returns>A simple list of factors.</returns>
    public static List<int> Factor(int i)
    {
        var primeIndex = 0;
        var prime = PrimeTable[primeIndex];
        var factors = new List<int>();

        while (i > 1)
        {
            var divResult = Math.DivRem(i, prime, out var remainder);

            if (remainder == 0)
            {
                factors.Add(prime);
                i = divResult;
            }
            else
            {
                ++primeIndex;
                prime = PrimeTable[primeIndex];
            }
        }

        return factors;
    }

    /// <summary>
    /// Given two integers expressed as a list of prime factors, divides these numbers
    /// and returns an integer also expressed as a set of prime factors.
    /// If the result is not a integer, then the result is undefined.  That is, 11 / 5
    /// when divided by this function will not yield a correct result.
    /// As such, this function is ONLY useful for division with combinatorial results where
    /// the result is known to be an integer AND the division occurs as the last operation(s).
    /// </summary>
    /// <param name="numerator">Numerator argument, expressed as list of prime factors.</param>
    /// <param name="denominator">Denominator argument, expressed as list of prime factors.</param>
    /// <returns>Resultant, expressed as list of prime factors.</returns>
    public static List<int> DividePrimeFactors(IEnumerable<int> numerator, IEnumerable<int> denominator)
    {
        _ = numerator ?? throw new ArgumentNullException(nameof(numerator));
        _ = denominator ?? throw new ArgumentNullException(nameof(denominator));
        var product = numerator.ToList();
        foreach (var prime in denominator)
            product.Remove(prime);
        return product;
    }

    /// <summary>
    /// Given a list of prime factors returns the long representation.
    /// </summary>
    /// <param name="value">Integer, expressed as list of prime factors.</param>
    /// <returns>Standard long representation.</returns>
    public static BigInteger EvaluatePrimeFactors(IEnumerable<int> value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));
        BigInteger result = 1;
        foreach (var prime in value)
            result *= prime;
        return result;
    }

    /// <summary>
    /// Static initializer, set up prime table.
    /// </summary>
    static SmallPrimeUtility()
    {
        PrimeTable = CalculatePrimes();
    }

    /// <summary>
    /// Calculate all primes up to Sqrt(2^32) = 2^16.
    /// This table will be large enough for all factorizations for Int32's.
    /// Small tables are best built using the Sieve Of Eratosthenes,
    /// Reference: http://primes.utm.edu/glossary/page.php?sort=SieveOfEratosthenes
    /// </summary>
    private static IReadOnlyList<int> CalculatePrimes()
    {
        // Build Sieve Of Eratosthenes
        var sieve = new BitArray(65536, true);
        for (var possiblePrime = 2; possiblePrime <= 256; ++possiblePrime)
        {
            if (!sieve[possiblePrime]) continue;

            // It is prime, so remove all future factors...
            for (var nonPrime = 2 * possiblePrime; nonPrime < 65536; nonPrime += possiblePrime)
                sieve[nonPrime] = false;
        }

        // Scan sieve for primes...
        var primes = new List<int>();
        for (var i = 2; i < 65536; ++i)
        {
            if (sieve[i])
                primes.Add(i);
        }

        return primes;
    }

    /// <summary>
    /// A List of all primes from 2 to 2^16.
    /// </summary>
    public static IReadOnlyList<int> PrimeTable { get; }
}
