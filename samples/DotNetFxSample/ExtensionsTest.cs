using System;
using WeihanLi.Extensions;

namespace DotNetFxSample
{
    public class ExtensionsTest
    {
        public static void IsPrimaryTest()
        {
            var types = new[]
            {
                typeof(bool),

                typeof(sbyte),
                typeof(byte),
                typeof(int),
                typeof(uint),
                typeof(short),
                typeof(ushort),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),

                typeof(DateTime),// IsPrimitive:False
                typeof(TimeSpan),// IsPrimitive:False

                typeof(char),
                typeof(string),// IsPrimitive:False

                //typeof(object),// IsPrimitive:False
            };
            types.ForEach(t =>
            {
                Console.WriteLine($"{t.FullName}");
                Console.WriteLine($"IsPrimitive:{t.IsPrimitive}");
                Console.WriteLine($"DefaultValue:{t.GetDefaultValue()}");
                Console.WriteLine($"Serilize Result:{t.GetDefaultValue().ToJson()}");
                Console.WriteLine($"IsBasicType:{t.IsBasicType()}");
                Console.WriteLine();
            });
        }
    }
}
