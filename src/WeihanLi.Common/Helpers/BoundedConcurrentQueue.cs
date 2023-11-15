using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Helpers;

public sealed class BoundedConcurrentQueue<T>
{
    private const int NonBounded = -1;
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly int _queueLimit;
    private readonly BoundedQueueFullMode _mode;
    private int _counter;

    public BoundedConcurrentQueue()
    {
        _queueLimit = NonBounded;
        _mode = BoundedQueueFullMode.DropWrite;
    }

    public BoundedConcurrentQueue(int queueLimit, BoundedQueueFullMode mode = BoundedQueueFullMode.DropWrite)
    {
        if (queueLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(queueLimit), Resource.ValueMustBePositive);

        _queueLimit = queueLimit;
        _mode = mode;
    }

    public int Count => _queue.Count;

    public bool TryDequeue([NotNullWhen(true)]out T item)
    {
        if (_queueLimit == NonBounded)
            return _queue.TryDequeue(out item);

        var result = false;
        
        if (_queue.TryDequeue(out item))
        {
            result = true;
            Interlocked.Decrement(ref _counter);
        }

        return result;
    }

    public bool TryEnqueue(T item)
    {
        if (_queueLimit == NonBounded)
        {
            _queue.Enqueue(item);
            return true;
        }

        var result = true;
        
        if (Interlocked.Increment(ref _counter) <= _queueLimit)
        {
            _queue.Enqueue(item);
        }
        else
        {
            if (_mode == BoundedQueueFullMode.DropOldest)
            {
                while (Interlocked.Decrement(ref _counter) >= _queueLimit)
                {
                    _queue.TryDequeue(out _);
                }
                _queue.Enqueue(item);
                result = true;
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

public enum BoundedQueueFullMode
{
    DropWrite = 0,
    DropOldest = 1
}
