using System;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspects
    {
        internal static readonly FluentAspectOptions AspectOptions = new FluentAspectOptions();

        public static void Configure(Action<FluentAspectOptions> optionsAction)
        {
            optionsAction?.Invoke(AspectOptions);
        }
    }
}
