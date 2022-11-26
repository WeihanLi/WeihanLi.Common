// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

public sealed class ContextAccessor<TContext> where TContext: class
{
    private static readonly AsyncLocal<ContextHolder<TContext>> ContextCurrent = new();
    
    public static TContext? Context
    {
        get
        {
            return ContextCurrent.Value?.Value;
        }
        set
        {
            var holder = ContextCurrent.Value;
            if (holder != null)
            {
                // Clear current Value trapped in the AsyncLocals, as its done.
                holder.Value = null;
            }
            if (value != null)
            {
                // Use an object indirection to hold the Value in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                ContextCurrent.Value = new ContextHolder<TContext>
                {
                    Value = value
                };
            }
        }
    }
}

file sealed class ContextHolder<TContext> where TContext: class
{
    public TContext? Value;
}
