using WeihanLi.Common.Models;

namespace DotNetCoreSample;

public class PagedListResultTest
{
    public static void MainTest()
    {
        var listResult = new ListResultWithTotal<int>()
        {
            Data = Enumerable.Range(0, 10).ToArray(),
            TotalCount = 20,
        };
        // GetEnumerator extensions for foreach
        foreach (var res in listResult)
        {
            Console.WriteLine(res);
        }
    }
}
