using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Helpers;

public interface IEnricher<in TContext>
{
    void Enrich(TContext context);
}

public abstract class PropertyEnricher<TContext> : IEnricher<TContext>
{
    private readonly string _propertyName;
    private readonly Func<TContext, object> _propertyValueFactory;
    private readonly bool _overwrite;
    private readonly Func<TContext, bool>? _propertyPredict;

    /// <summary>
    /// EnrichAction
    /// (context,propertyName,propertyValueFactory,overwrite)=>{}
    /// </summary>
    protected abstract Action<TContext, string, Func<TContext, object>, bool>? EnrichAction
    {
        get;
    }

    protected PropertyEnricher(string propertyName, object propertyValue, bool overwrite = false) : this(propertyName, _ => propertyValue, overwrite)
    {
    }

    protected PropertyEnricher(string propertyName, Func<TContext, object> propertyValueFactory,
        bool overwrite = false) : this(propertyName, propertyValueFactory, null, overwrite)
    {
    }

    protected PropertyEnricher(string propertyName, Func<TContext, object> propertyValueFactory, Func<TContext, bool>? propertyPredict,
        bool overwrite = false)
    {
        _propertyName = propertyName;
        _propertyValueFactory = propertyValueFactory;
        _propertyPredict = propertyPredict;
        _overwrite = overwrite;
    }

    public void Enrich(TContext context)
    {
        if (EnrichAction != null && _propertyPredict?.Invoke(context) != false)
        {
            EnrichAction.Invoke(context, _propertyName, _propertyValueFactory, _overwrite);
        }
    }
}

public interface IAsyncEnricher<in TContext>
{
    Task Enrich(TContext context);
}

public abstract class AsyncPropertyEnricher<TContext> : IAsyncEnricher<TContext>
{
    private readonly string _propertyName;
    private readonly Func<TContext, object>? _propertyValueFactory;
    private readonly bool _overwrite;
    private readonly Func<TContext, bool>? _propertyPredict;

    protected abstract Func<TContext, string, Func<TContext, object>?, bool, Task>? EnrichAction
    {
        get;
    }

    protected AsyncPropertyEnricher(string propertyName, object propertyValue, bool overwrite = false) : this(propertyName, _ => propertyValue, overwrite)
    {
    }

    protected AsyncPropertyEnricher(string propertyName, Func<TContext, object>? propertyValueFactory,
        bool overwrite = false) : this(propertyName, propertyValueFactory, null, overwrite)
    {
    }

    protected AsyncPropertyEnricher(string propertyName, Func<TContext, object>? propertyValueFactory, Func<TContext, bool>? propertyPredict,
        bool overwrite = false)
    {
        _propertyName = propertyName;
        _propertyValueFactory = propertyValueFactory;
        _propertyPredict = propertyPredict;
        _overwrite = overwrite;
    }

    public async Task Enrich(TContext context)
    {
        if (EnrichAction != null && _propertyPredict?.Invoke(context) != false)
        {
            await EnrichAction.Invoke(context, _propertyName, _propertyValueFactory, _overwrite);
        }
    }
}
