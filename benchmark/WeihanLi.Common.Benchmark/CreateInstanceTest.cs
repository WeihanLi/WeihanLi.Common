using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Benchmark
{
    public class CreateInstanceTest
    {
        [Benchmark]
        public MapperTest.B NewInstanceByExpression()
        {
            return Expression.Lambda<Func<MapperTest.B>>(Expression.New(typeof(MapperTest.B))).Compile().Invoke();
        }

        [Benchmark]
        public MapperTest.B NewInstanceByReflection()
        {
            return Activator.CreateInstance<MapperTest.B>();
        }

        [Benchmark]
        public MapperTest.B NewInstanceByActivatorHelper()
        {
            return ActivatorHelper.CreateInstance<MapperTest.B>(DependencyResolver.Current);
        }

        [Benchmark]
        public MapperTest.B NewInstance()
        {
            return new MapperTest.B();
        }
    }
}
