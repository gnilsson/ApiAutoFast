namespace ApiAutoFast;

public class PairCollection<T1, T2> : List<Pair<T1, T2>>
{
    private readonly T1? _defaultFirst;
    private readonly T2? _defaultLast;

    public PairCollection(T1? defaultFirst = default, T2? defaultLast = default)
    {
        _defaultFirst = defaultFirst;
        _defaultLast = defaultLast;
    }

    public T2? this[T1 key]
    {
        get
        {
            var pair = this.SingleOrDefault(x => x.First!.Equals(key));
            if (pair is null) return _defaultLast;
            return pair.Last;
        }
    }

    public T1? this[T2 key]
    {
        get
        {
            var pair = this.SingleOrDefault(x => x.Last!.Equals(key));
            if (pair is null) return _defaultFirst;
            return pair.First;
        }
    }
}
