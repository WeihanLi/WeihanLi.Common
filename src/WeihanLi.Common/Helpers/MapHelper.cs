using System;
using System.Linq;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public static class MapHelper
    {
        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);

            var properties = CacheUtil.GetTypeProperties(destinationType);
            var sourceProps = CacheUtil.GetTypeProperties(sourceType)
                .Where(x => properties.Any(_ => _.Name.EqualsIgnoreCase(x.Name)))
                .ToArray();

            var result = new TTarget();

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceProps.FirstOrDefault(p => p.Name.EqualsIgnoreCase(property.Name));
                    if (sourceProperty == null)
                    {
                        continue;
                    }

                    var propGetter = sourceProperty.GetValueGetter();
                    if (propGetter != null)
                    {
                        property.GetValueSetter()?.Invoke(result, propGetter.Invoke(source));
                    }
                }
            }
            return result;
        }

        public static TTarget MapWith<TSource, TTarget>(TSource source, params string[] propertiesToMap) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);
            var result = new TTarget();
            var properties = CacheUtil.GetTypeProperties(destinationType)
                .Where(p => propertiesToMap.Any(_ => string.Equals(_, p.Name, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
            var sourceProps = CacheUtil.GetTypeProperties(sourceType)
                .Where(x => propertiesToMap.Any(_ => _.EqualsIgnoreCase(x.Name)))
                .ToArray();

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceProps.FirstOrDefault(p => p.Name.EqualsIgnoreCase(property.Name));
                    if (sourceProperty == null || !sourceProperty.CanRead || !property.CanWrite)
                    {
                        continue;
                    }

                    var propGetter = sourceProperty.GetValueGetter();
                    if (propGetter != null)
                    {
                        property.GetValueSetter()?.Invoke(result, propGetter.Invoke(source));
                    }
                }
            }

            return result;
        }

        public static TTarget MapWithout<TSource, TTarget>(TSource source, params string[] propertiesNoMap) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);

            var properties = CacheUtil.GetTypeProperties(destinationType)
                .Where(p => !propertiesNoMap.Any(_ => string.Equals(_, p.Name, StringComparison.Ordinal)))
                .ToArray();
            var sourceProps = CacheUtil.GetTypeProperties(sourceType)
                .Where(x => !properties.Any(_ => _.Name.EqualsIgnoreCase(x.Name)))
                .ToArray();

            var result = new TTarget();

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceProps.FirstOrDefault(p => p.Name.EqualsIgnoreCase(property.Name));
                    if (sourceProperty == null || !sourceProperty.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    var propGetter = sourceProperty.GetValueGetter();
                    if (propGetter != null)
                    {
                        property.GetValueSetter()?.Invoke(result, propGetter.Invoke(source));
                    }
                }
            }

            return result;
        }
    }
}
