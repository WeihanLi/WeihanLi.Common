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
            Console.WriteLine(abc.JsonToObject<string>());

            var integer = 1;
            var a = integer.ToJson();
            Console.WriteLine(a.JsonToObject<int>());

            var time = DateTime.Now;
            var t = time.ToJson();
            var t1 = t.JsonToObject<DateTime>();
            Console.WriteLine(t1);
        }
    }
}
