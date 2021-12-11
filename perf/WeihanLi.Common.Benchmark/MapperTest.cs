using AutoMapper;
using BenchmarkDotNet.Attributes;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Benchmark;

public class MapperTest
{
    private readonly A _source;

    static MapperTest()
    {
        Mapper.Initialize(cfg => cfg.CreateMap<A, B>());
    }

    public MapperTest()
    {
        _source = new A
        {
            Name = "张三",
            Age = 10,
            Class = "一班",
            CObject = new SubC()
            {
                Message = "Hello"
            },
            P1 = "1",
            P2 = "2",
            P3 = "3",
            P4 = "4",
            P5 = "5",
            P6 = "6"
        };
    }

    [Benchmark(Baseline = true)]
    public B AutoMapperBenchmark()
    {
        return Mapper.Map<B>(_source);
    }

    [Benchmark]
    public B MapHelperBenchmark()
    {
        return MapHelper.Map<A, B>(_source);
    }

    #region TestType

    public class A
    {
        public string Name
        {
            get; set;
        }

        public int Age
        {
            get; set;
        }

        public string Class
        {
            get; set;
        }

        public SubC CObject
        {
            get; set;
        }

        public string P1
        {
            get; set;
        }

        public string P2
        {
            get; set;
        }

        public string P3
        {
            get; set;
        }

        public string P4
        {
            get; set;
        }

        public string P5
        {
            get; set;
        }

        public string P6
        {
            get; set;
        }
    }

    public class B
    {
        public string Name
        {
            get; set;
        }

        public int Age
        {
            get; set;
        }

        public C CObject
        {
            get; set;
        }

        public string P1
        {
            get; set;
        }

        public string P2
        {
            get; set;
        }

        public string P3
        {
            get; set;
        }

        public string P4
        {
            get; set;
        }

        public string P5
        {
            get; set;
        }

        public string P6
        {
            get; set;
        }
    }

    public class C
    {
        public string Message
        {
            get; set;
        }
    }

    public class SubC : C
    {
    }

    #endregion TestType
}
