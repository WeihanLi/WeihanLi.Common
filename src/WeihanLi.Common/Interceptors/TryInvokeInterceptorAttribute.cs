using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Interceptors
{
    /// <summary>
    /// TryInvoke
    /// </summary>
    public class TryInvokeInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
        }
    }
}
