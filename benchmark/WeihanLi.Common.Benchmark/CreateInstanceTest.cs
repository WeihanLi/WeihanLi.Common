using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

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
        public MapperTest.B NewInstance()
        {
            return new MapperTest.B();
        }
    }
}
