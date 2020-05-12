using System;

namespace WeihanLi.Common.Aspect
{
    public interface IInvocationEnricher
    {
        void Enrich(IInvocation invocation);
    }

    public sealed class PropertyInvocationEnricher : IInvocationEnricher
    {
        private readonly string _propertyName;
        private readonly Func<IInvocation, object> _propertyValueFactory;
        private readonly bool _overwrite;
        private readonly Func<IInvocation, bool> _propertyPredict;

        public PropertyInvocationEnricher(string propertyName, object propertyValue, bool overwrite = false) : this(propertyName, invocation => propertyValue, overwrite)
        {
        }

        public PropertyInvocationEnricher(
            string propertyName,
            Func<IInvocation, object> propertyValueFactory,
            bool overwrite = false
            ) : this(
                propertyName,
                propertyValueFactory,
                null,
                overwrite)
        {
        }

        public PropertyInvocationEnricher(
            string propertyName,
            Func<IInvocation, object> propertyValueFactory,
            Func<IInvocation, bool> propertyPredict,
            bool overwrite = false)
        {
            _propertyName = propertyName;
            _propertyValueFactory = propertyValueFactory;
            _propertyPredict = propertyPredict;
            _overwrite = overwrite;
        }

        public void Enrich(IInvocation invocation)
        {
            if (_propertyPredict?.Invoke(invocation) != false)
            {
                if (_overwrite || !invocation.Properties.ContainsKey(_propertyName))
                {
                    invocation.Properties[_propertyName] = _propertyValueFactory.Invoke(invocation);
                }
            }
        }
    }
}
