using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Benchmark
{
    public class CreateInstanceTest
    {
        private static readonly Lazy<Func<MapperTest.B>> lazyFunc = new Lazy<Func<MapperTest.B>>(
            Expression.Lambda<Func<MapperTest.B>>(Expression.New(typeof(MapperTest.B))).Compile()
            );

        [Benchmark]
        public MapperTest.B NewInstanceByExpression()
        {
            return lazyFunc.Value.Invoke();
        }

        [Benchmark]
        public MapperTest.B NewInstanceByReflection()
        {
            return Activator.CreateInstance<MapperTest.B>();
        }

        [Benchmark]
        public MapperTest.B NewInstanceByActivatorHelper()
        {
            return ActivatorHelper.CreateInstance<MapperTest.B>();
        }

        [Benchmark]
        public MapperTest.B NewInstance()
        {
            return new MapperTest.B();
        }
    }
}
