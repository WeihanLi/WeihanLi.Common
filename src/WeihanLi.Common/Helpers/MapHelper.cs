using System;
using System.Linq;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// Mapper
    /// </summary>
    public static class MapHelper
    {
        /// <summary>
        /// 对象属性拷贝
        /// </summary>
        /// <typeparam name="TSource">SourceType</typeparam>
        /// <typeparam name="TTarget">TargetType</typeparam>
        /// <param name="source">source</param>
        /// <returns>destination</returns>
        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);

            var properties = CacheUtil.GetTypeProperties(destinationType);

            var result = new TTarget();

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceType.GetProperty(property.Name);
                    if (sourceProperty == null || !sourceProperty.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    property.SetValue(result, sourceProperty.GetValue(source));
                }
            }
            return result;
        }

        /// <summary>
        /// 对象属性拷贝，拷贝指定属性
        /// </summary>
        /// <typeparam name="TSource">SourceType</typeparam>
        /// <typeparam name="TTarget">TargetType</typeparam>
        /// <param name="source">source</param>
        /// <param name="propertiesToMap">propertiesToMap</param>
        /// <returns>destination</returns>
        public static TTarget MapWith<TSource, TTarget>(TSource source, params string[] propertiesToMap) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);
            var result = new TTarget();

            var properties = CacheUtil.GetTypeProperties(destinationType).Where(p => propertiesToMap.Any(_ => string.Equals(_, p.Name, StringComparison.Ordinal))).ToArray();
            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceType.GetProperty(property.Name);
                    if (sourceProperty == null || !sourceProperty.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    property.SetValue(result, sourceProperty.GetValue(source));
                }
            }

            return result;
        }

        /// <summary>
        /// 对象属性拷贝，不拷贝指定属性
        /// </summary>
        /// <typeparam name="TSource">SourceType</typeparam>
        /// <typeparam name="TTarget">TargetType</typeparam>
        /// <param name="source">source</param>
        /// <param name="propertiesNoMap">propertiesNoMap</param>
        /// <returns>destination</returns>
        public static TTarget MapWithout<TSource, TTarget>(TSource source, params string[] propertiesNoMap) where TTarget : new()
        {
            if (source == null)
            {
                return (TTarget)(object)null;
            }

            var sourceType = typeof(TSource);
            var destinationType = typeof(TTarget);

            var properties = CacheUtil.GetTypeProperties(destinationType).Where(p => propertiesNoMap.Any(_ => !string.Equals(_, p.Name, StringComparison.Ordinal))).ToArray();

            var result = new TTarget();

            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var sourceProperty = sourceType.GetProperty(property.Name);
                    if (sourceProperty == null || !sourceProperty.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    property.SetValue(result, sourceProperty.GetValue(source));
                }
            }

            return result;
        }
    }
}
