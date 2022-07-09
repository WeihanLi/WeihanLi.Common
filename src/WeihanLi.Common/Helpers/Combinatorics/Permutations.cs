// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections;
using System.Numerics;

namespace WeihanLi.Common.Helpers.Combinatorics;

/// <summary>
/// Permutations defines a sequence of all possible orderings of a set of values.
/// </summary>
/// <remarks>
/// When given a input collect {A A B}, the following sets are generated:
/// MetaCollectionType.WithRepetition =>
/// {A A B}, {A B A}, {A A B}, {A B A}, {B A A}, {B A A}
/// MetaCollectionType.WithoutRepetition =>
/// {A A B}, {A B A}, {B A A}
///
/// When generating non-repetition sets, ordering is based on the lexicographic
/// ordering of the lists based on the provided Comparer.
/// If no comparer is provided, then T must be IComparable on T.
///
/// When generating repetition sets, no comparisons are performed and therefore
/// no comparer is required and T does not need to be IComparable.
/// </remarks>
/// <typeparam name="T">The type of the values within the list.</typeparam>
public sealed class Permutations<T> : IEnumerable<IReadOnlyList<T>>
{
    /// <summary>
    /// Create a permutation set from the provided list of values.
    /// The values (T) must implement IComparable.
    /// If T does not implement IComparable use a constructor with an explicit IComparer.
    /// The repetition type defaults to MetaCollectionType.WithholdRepetitionSets
    /// </summary>
    /// <param name="values">List of values to permute.</param>
    public Permutations(IEnumerable<T> values)
        : this(values, GenerateOption.WithoutRepetition, null)
    {
    }

    /// <summary>
    /// Create a permutation set from the provided list of values.
    /// If type is MetaCollectionType.WithholdRepetitionSets, then values (T) must implement IComparable.
    /// If T does not implement IComparable use a constructor with an explicit IComparer.
    /// </summary>
    /// <param name="values">List of values to permute.</param>
    /// <param name="type">The type of permutation set to calculate.</param>
    public Permutations(IEnumerable<T> values, GenerateOption type)
        : this(values, type, null)
    {
    }

    /// <summary>
    /// Create a permutation set from the provided list of values.
    /// The values will be compared using the supplied IComparer.
    /// The repetition type defaults to MetaCollectionType.WithholdRepetitionSets
    /// </summary>
    /// <param name="values">List of values to permute.</param>
    /// <param name="comparer">Comparer used for defining the lexicographic order.</param>
    public Permutations(IEnumerable<T> values, IComparer<T>? comparer)
        : this(values, GenerateOption.WithoutRepetition, comparer)
    {
    }

    /// <summary>
    /// Create a permutation set from the provided list of values.
    /// If type is MetaCollectionType.WithholdRepetitionSets, then the values will be compared using the supplied IComparer.
    /// </summary>
    /// <param name="values">List of values to permute.</param>
    /// <param name="type">The type of permutation set to calculate.</param>
    /// <param name="comparer">Comparer used for defining the lexicographic order.</param>
    public Permutations(IEnumerable<T> values, GenerateOption type, IComparer<T>? comparer)
    {
        _ = values ?? throw new ArgumentNullException(nameof(values));

        // Copy information provided and then create a parallel int array of lexicographic
        // orders that will be used for the actual permutation algorithm.
        // The input array is first sorted as required for WithoutRepetition and always just for consistency.
        // This array is constructed one of two way depending on the type of the collection.
        //
        // When type is MetaCollectionType.WithRepetition, then all N! permutations are returned
        // and the lexicographic orders are simply generated as 1, 2, ... N.
        // E.g.
        // Input array:          {A A B C D E E}
        // Lexicographic Orders: {1 2 3 4 5 6 7}
        //
        // When type is MetaCollectionType.WithoutRepetition, then fewer are generated, with each
        // identical element in the input array not repeated.  The lexicographic sort algorithm
        // handles this natively as long as the repetition is repeated.
        // E.g.
        // Input array:          {A A B C D E E}
        // Lexicographic Orders: {1 1 2 3 4 5 5}

        Type = type;
        _myValues = values as T[] ?? values.ToArray();
        _myLexicographicOrders = new int[_myValues.Length];

        if (type == GenerateOption.WithRepetition)
        {
            for (var i = 0; i < _myLexicographicOrders.Length; ++i)
            {
                _myLexicographicOrders[i] = i;
            }
        }
        else
        {
            comparer ??= Comparer<T>.Default;

            Array.Sort(_myValues, comparer);

            var j = 1;
            if (_myLexicographicOrders.Length > 0)
            {
                _myLexicographicOrders[0] = j;
            }

            for (var i = 1; i < _myLexicographicOrders.Length; ++i)
            {
                if (comparer.Compare(_myValues[i - 1], _myValues[i]) != 0)
                {
                    ++j;
                }

                _myLexicographicOrders[i] = j;
            }
        }

        Count = GetCount();
    }

    /// <summary>
    /// Gets an enumerator for collecting the list of permutations.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<IReadOnlyList<T>> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The enumerator that enumerates each meta-collection of the enclosing Permutations class.
    /// </summary>
    internal sealed class Enumerator : IEnumerator<IReadOnlyList<T>>
    {
        /// <summary>
        /// Construct a enumerator with the parent object.
        /// </summary>
        /// <param name="source">The source Permutations object.</param>
        public Enumerator(Permutations<T> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _myParent = source;
            _myLexicographicalOrders = new int[source._myLexicographicOrders.Length];
            _myValues = new List<T>(source._myValues.Length);
            source._myLexicographicOrders.CopyTo(_myLexicographicalOrders, 0);
            _myPosition = Position.BeforeFirst;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        /// <summary>
        /// Advances to the next permutation.
        /// </summary>
        /// <returns>True if successfully moved to next permutation, False if no more permutations exist.</returns>
        /// <remarks>
        /// Continuation was tried (i.e. yield return) by was not nearly as efficient.
        /// Performance is further increased by using value types and removing generics, that is, the LexicographicOrder parellel array.
        /// This is a issue with the .NET CLR not optimizing as well as it could in this infrequently used scenario.
        /// </remarks>
        public bool MoveNext()
        {
            switch (_myPosition)
            {
                case Position.BeforeFirst:
                    _myValues.AddRange(_myParent._myValues);
                    _myPosition = Position.InSet;
                    break;

                case Position.InSet:
                    if (_myValues.Count < 2)
                    {
                        _myPosition = Position.AfterLast;
                    }
                    else if (!NextPermutation())
                    {
                        _myPosition = Position.AfterLast;
                    }
                    break;

                case Position.AfterLast:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return _myPosition != Position.AfterLast;
        }

        object IEnumerator.Current => Current;

        /// <summary>
        /// The current permutation.
        /// </summary>
        public IReadOnlyList<T> Current
        {
            get
            {
                if (_myPosition == Position.InSet)
                    return new List<T>(_myValues);

                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Calculates the next lexicographical permutation of the set.
        /// This is a permutation with repetition where values that compare as equal will not
        /// swap positions to create a new permutation.
        /// http://www.cut-the-knot.org/do_you_know/AllPerm.shtml
        /// E. W. Dijkstra, A Discipline of Programming, Prentice-Hall, 1997
        /// </summary>
        /// <returns>True if a new permutation has been returned, false if not.</returns>
        /// <remarks>
        /// This uses the integers of the lexicographical order of the values so that any
        /// comparison of values are only performed during initialization.
        /// </remarks>
        private bool NextPermutation()
        {
            var i = _myLexicographicalOrders.Length - 1;

            while (_myLexicographicalOrders[i - 1] >= _myLexicographicalOrders[i])
            {
                --i;
                if (i == 0)
                {
                    return false;
                }
            }

            var j = _myLexicographicalOrders.Length;

            while (_myLexicographicalOrders[j - 1] <= _myLexicographicalOrders[i - 1])
            {
                --j;
            }

            Swap(i - 1, j - 1);

            ++i;

            j = _myLexicographicalOrders.Length;

            while (i < j)
            {
                Swap(i - 1, j - 1);
                ++i;
                --j;
            }
            return true;
        }

        /// <summary>
        /// Helper function for swapping two elements within the internal collection.
        /// This swaps both the lexicographical order and the values, maintaining the parallel array.
        /// </summary>
        private void Swap(int i, int j)
        {
            var temp = _myValues[i];
            _myValues[i] = _myValues[j];
            _myValues[j] = temp;
            _myKviTemp = _myLexicographicalOrders[i];
            _myLexicographicalOrders[i] = _myLexicographicalOrders[j];
            _myLexicographicalOrders[j] = _myKviTemp;
        }

        /// <summary>
        /// Single instance of swap variable for int, small performance improvement over declaring in Swap function scope.
        /// </summary>
        private int _myKviTemp;

        /// <summary>
        /// Flag indicating the position of the enumerator.
        /// </summary>
        private Position _myPosition;

        /// <summary>
        /// Parallel array of integers that represent the location of items in the myValues array.
        /// This is generated at Initialization and is used as a performance speed up rather that
        /// comparing T each time, much faster to let the CLR optimize around integers.
        /// </summary>
        private readonly int[] _myLexicographicalOrders;

        /// <summary>
        /// The list of values that are current to the enumerator.
        /// </summary>
        private readonly List<T> _myValues;

        /// <summary>
        /// The set of permutations that this enumerator enumerates.
        /// </summary>
        private readonly Permutations<T> _myParent;

        /// <summary>
        /// Internal position type for tracking enumerator position.
        /// </summary>
        private enum Position
        {
            BeforeFirst,
            InSet,
            AfterLast
        }
    }

    /// <summary>
    /// The count of all permutations that will be returned.
    /// If <see cref="Type"/> is <see cref="GenerateOption.WithoutRepetition"/>, then this does not count equivalent result sets.
    /// I.e., count of permutations of "AAB" will be 3 instead of 6.
    /// If <see cref="Type"/> is <see cref="GenerateOption.WithRepetition"/>, then this is all combinations and is therefore N!, where N is the number of values in the input set.
    /// </summary>
    public BigInteger Count { get; }

    /// <summary>
    /// The type of permutations set that is generated.
    /// </summary>
    public GenerateOption Type { get; }

    /// <summary>
    /// The upper index of the meta-collection, equal to the number of items in the input set.
    /// </summary>
    public int UpperIndex => _myValues.Length;

    /// <summary>
    /// The lower index of the meta-collection, equal to the number of items returned each iteration.
    /// This is always equal to <see cref="UpperIndex"/>.
    /// </summary>
    public int LowerIndex => _myValues.Length;

    /// <summary>
    /// Calculates the total number of permutations that will be returned.
    /// As this can grow very large, extra effort is taken to avoid overflowing the accumulator.
    /// While the algorithm looks complex, it really is just collecting numerator and denominator terms
    /// and cancelling out all of the denominator terms before taking the product of the numerator terms.
    /// </summary>
    /// <returns>The number of permutations.</returns>
    private BigInteger GetCount()
    {
        var runCount = 1;
        var divisors = Enumerable.Empty<int>();
        var numerators = Enumerable.Empty<int>();

        for (var i = 1; i < _myLexicographicOrders.Length; ++i)
        {
            numerators = numerators.Concat(SmallPrimeUtility.Factor(i + 1));

            if (_myLexicographicOrders[i] == _myLexicographicOrders[i - 1])
            {
                ++runCount;
            }
            else
            {
                for (var f = 2; f <= runCount; ++f)
                    divisors = divisors.Concat(SmallPrimeUtility.Factor(f));

                runCount = 1;
            }
        }

        for (var f = 2; f <= runCount; ++f)
            divisors = divisors.Concat(SmallPrimeUtility.Factor(f));

        return SmallPrimeUtility.EvaluatePrimeFactors(
            SmallPrimeUtility.DividePrimeFactors(numerators, divisors)
        );
    }

    /// <summary>
    /// A list of T that represents the order of elements as originally provided.
    /// </summary>
    private readonly T[] _myValues;

    /// <summary>
    /// Parallel array of integers that represent the location of items in the myValues array.
    /// This is generated at Initialization and is used as a performance speed up rather that
    /// comparing T each time, much faster to let the CLR optimize around integers.
    /// </summary>
    private readonly int[] _myLexicographicOrders;
}
