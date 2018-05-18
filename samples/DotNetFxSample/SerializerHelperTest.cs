using System;
using WeihanLi.Extensions;

namespace DotNetFxSample
{
    internal class SerializerHelperTest
    {
        public static void JsonSerializeTest()
        {
            var str = "abc";
            var abc = str.ToJson();
            var abc1 = str.ToJsonOrString();
            Console.WriteLine(abc);
            Console.WriteLine(abc1);
            Console.WriteLine(abc.JsonToType<string>());

            var integer = 1;
            var a = integer.ToJson();
            Console.WriteLine(a.JsonToType<int>());

            var time = DateTime.Now;
            var t = time.ToJson();
            var t1 = t.JsonToType<DateTime>();
            Console.WriteLine(t1);
        }
    }
}
