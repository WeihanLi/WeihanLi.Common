using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WeihanLi.Common.Helpers;

public class BoundedConcurrentQueue<T>
{
    private const int NON_BOUNDED = -1;
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _queueLimit;
    private int _counter;

    public BoundedConcurrentQueue()
    {
        _queueLimit = NON_BOUNDED;
    }

    public BoundedConcurrentQueue(int queueLimit)
    {
        if (queueLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(queueLimit), Resource.ValueMustBePositive);

        _queueLimit = queueLimit;
    }

    public int Count => _queue.Count;

    public bool TryDequeue(out T? item)
    {
        if (_queueLimit == NON_BOUNDED)
            return _queue.TryDequeue(out item);

        var result = false;
        try
        { }
        finally // prevent state corrupt while aborting
        {
            if (_queue.TryDequeue(out item))
            {
                Interlocked.Decrement(ref _counter);
                result = true;
            }
        }

        return result;
    }

    public bool TryEnqueue(T item)
    {
        if (_queueLimit == NON_BOUNDED)
        {
            _queue.Enqueue(item);
            return true;
        }

        var result = true;
        try
        { }
        finally
        {
            if (Interlocked.Increment(ref _counter) <= _queueLimit)
            {
                _queue.Enqueue(item);
            }
            else
            {
                Interlocked.Decrement(ref _counter);
                result = false;
            }
        }

        return result;
    }
}
