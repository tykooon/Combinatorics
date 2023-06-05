using System.Collections;
using System.Text;

namespace Permutations;

/// <summary>
/// Represent a permutation as array of int
/// </summary>
public struct Permutation : IEnumerable<int>, IEnumerable, IEquatable<Permutation>, IComparable<Permutation>
{
    private readonly int[] _permutation;

    /// <summary>
    /// Represents the order of permutation. 
    /// </summary>
    public int Count => _permutation.Length;
    /// <summary>
    /// Returns element with specified index
    /// </summary>
    /// <param name="index"> index of element in permutation: from 0 to <see cref="Count"/> (not included) </param>
    /// <returns></returns>
    public int this[int index] => _permutation[index];

    /// <summary>
    /// Creates a permutation object
    /// </summary>
    /// <param name="permutation">Unordered array of integers from 1 to N, where N is length of the array</param>
    /// <exception cref="ArgumentException"> In case of wrong array (with zeroes, negative numbers or missed ones).</exception>
    public Permutation(params int[] permutation)
    {
        if (!IsPermutaion(permutation))
        {
            throw new ArgumentException("Wrong permutaion");
        }
        _permutation = new int[permutation.Length];
        permutation.CopyTo(_permutation, 0);
    }

    public Permutation(IEnumerable<int> numbers) : this(numbers?.ToArray()) { }

    public Permutation Next()
    {
        var k = Count - 2;
        while (k != -1 && _permutation[k] > _permutation[k+1])
        {
            k--;
        }

        if (k == -1 )
        {
            return Identity(Count);
        }

        var next = new int[Count];
        _permutation.CopyTo(next, 0);

        var t = k + 1;
        while (t != Count && next[t] > next[k])
        {
            t++;
        }

        Swap(next, k, t - 1);
        for (var i = 1; i < (Count - k + 1) / 2; i++)
        {
            Swap(next, k + i, Count - i);
        }
        return new(next);
    }

    public Permutation Inverse() => new(Apply(Identity(Count)));

    public bool IsTransposition()
    {
        var changes = 0;
        for(var i = 0; i < Count; i++)
        {
            if (this[i] != i + 1) { changes++; }
        }
        return changes == 2;
    }

    public int Disorders()
    {
        var disorders = 0;
        for(var i = 0; i < Count - 1; i++)
        {
            for(var j= i + 1; j < Count; j++)
            {
                if (_permutation[i] > _permutation[j])
                {
                    disorders++;
                }
            }
        }
        return disorders;
    }

    public bool IsOdd()
    {
        return Disorders() % 2 != 0;
    }

    public Permutation Multiply(Permutation second)
    {
        CheckCount(second);
        var result = new int[Count];
        for (var i = 0; i < Count; i++)
        {
            result[i] = second[_permutation[i] -1 ];
        }
        return new(result);
    }

    public static Permutation Identity(int n)
    {
        return new(Enumerable.Range(1, n));
    }

    public static Permutation Random(int n)
    {
        var rnd = new Random();
        var res = new List<int>();
        var seed = new List<int>(Enumerable.Range(1, n));
        for (var i = 0; i < n - 1; i++)
        {
            int m = rnd.Next(n - i);
            res.Add(seed[m]);
            seed.Remove(seed[m]);
        }
        res.Add(seed[0]);
        return new Permutation(res);
    }

    public override string ToString()
    {
        var builder = new StringBuilder("(");
        _permutation.ToList().ForEach(x => builder.Append(x).Append(", "));
        return builder.ToString()[..^2] + ")";
    }

    public string ToString(string format)
    {
        return format switch
        {
            "cycle" => CycleToString(),
            "full" => "",
            _ => ToString()
        };
    }

    public string CycleToString()
    {
        var builder = new StringBuilder("(");
        for (var i = 0; i < Count; i++)
        {
            builder.Append(_permutation[i] != i + 1
                ? _permutation[i] + ", "
                : "_, "
                );
        }
        return builder.ToString()[..^2] + ")";
    }

    private static bool IsPermutaion(int[] array)
    {
        if (array.Length == 0)
        {
            return false;
        }
        return array.Order().SequenceEqual(Enumerable.Range(1, array.Length));
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return _permutation[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(Permutation other)
    {
        if (other.Count != Count)
        {
            return false;
        }
        return _permutation.ToList().SequenceEqual(other);
    }

    public int CompareTo(Permutation other)
    {
        if (Count != other.Count)
        {
            return Count - other.Count;
        }

        for (var i = 0; i < Count; i++)
        {
            if (this[i] - other[i] != 0)
            {
                return this[i] - other[i];
            }
        }
        return 0;
    }

    public override bool Equals(object obj) => 
        obj is Permutation other && Equals(other);


    public override int GetHashCode() => 
        this.Sum(x => x << 2);

    public static bool operator ==(Permutation left, Permutation right) =>
        left.Equals(right);

    public static bool operator !=(Permutation left, Permutation right) =>
        !(left == right);

    public static Permutation operator *(Permutation left, Permutation right) =>
        left.Multiply(right);

    public static bool operator <(Permutation left, Permutation right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(Permutation left, Permutation right) => 
        left.CompareTo(right) <= 0;

    public static bool operator >(Permutation left, Permutation right) => 
        left.CompareTo(right) > 0;

    public static bool operator >=(Permutation left, Permutation right) =>
        left.CompareTo(right) >= 0;

    public IEnumerable<Permutation> Factorization()
    {
        var res = new List<Permutation>();
        var temp = new Permutation(_permutation);
        var cycle = GetCycle();
        while (CycleLength(cycle) > 0)
        {
            res.Add(cycle);
            temp = temp.ReduceCycle(cycle);
            cycle = temp.GetCycle();
        }
        return res;
    }

    public static int CycleLength(Permutation cycle)
    {
        var res = 0;
        for (var i = 0; i < cycle.Count; i++)
        {
            if (cycle[i] != i + 1)
            {
                res++;
            }
        }
        return res;
    }

    private Permutation ReduceCycle(Permutation cycle)
    {
        CheckCount(cycle);
        var res = new int[Count];
        _permutation.CopyTo(res, 0);
        for(var i = 0; i < Count; i++)
        {
            if (cycle[i] != i + 1)
            {
                res[i] = i+1;
            }
        }
        return new Permutation(res);
    }

    private Permutation GetCycle()
    {
        var result = Enumerable.Range(1, Count).ToArray();
        var start = 0;
        while (start < Count && _permutation[start] == ++start)
        { }
        if (start == Count)
        {
            return new Permutation(result);
        }
        var end = --start;
        do
        {
            var index = result[start] = _permutation[start];
            //            Console.WriteLine(index);
            start = index;
        } while (end != --start);
        return new Permutation(result);
    }

    private static void Swap(int[] array, int firstIndex, int secondIndex) =>
        (array[firstIndex], array[secondIndex]) = (array[secondIndex], array[firstIndex]);

    private void CheckCount(Permutation other)
    {
        if (Count != other.Count)
        {
            throw new ArgumentException("Wrong permutation count.");
        }
    }

    public IEnumerable<T> Apply<T>(IEnumerable<T> inputList)
    {
        if (inputList == null || inputList.Count() != Count)
        {
            throw new ArgumentException("Wrong input data length.");
        }
        var result = new T[Count];
        var array = inputList.ToArray();
        for (var i = 0; i < Count; i++)
        {
            result[_permutation[i] - 1] = array[i];
        }
        return result;
    }
}
