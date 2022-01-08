using WeihanLi.Extensions;

namespace DotNetCoreSample;

public class Base64UrlEncodeTest
{
    private const string Jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxODI5NCIsIm5hbWUiOiJtdm4xNSIsInVuaXF1ZV9uYW1lIjoibXZuMTUiLCJuYmYiOjE1MzM1MzM1NzAsImV4cCI6MTUzNDM5NzU3MCwiaWF0IjoxNTMzNTMzNTcwLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjYzODI3LyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NjM4MjcvIn0.Quf78Ma-T5cLsLPy7VngYpRzVYY8T4YkLJ1Mg_A3VmeBJY-wUUzmH7YdYn6c8zet1g9U5xwQ44bPylwcxI8es7Swsvp9AmQLwgLWCr8pmZgCVbzlrE4o9sDzE7F0CBkkev1UMav2vZ0Ksy32hRCR9hQlco4ieBMV2x4PDiR937p3mQKGQQPI_hSHCHO40J-ELXsQqmlOSmV2sYqffKmLkV1UrUEGFj8nF9Gcm-fJhZtj3yo0-KojQhX7j7ZahG8HnL4D88rWpcsL1TFJNvlcJqPDy0DxZLe0C8UoVJWyKg6j6wqT9L_zfIk_HAd-OTqr7cen-F5j1JA7CsSbT5aWgQ";

    public static void MainTest()
    {
        Console.WriteLine($"Token:{Jwt}");
        Console.WriteLine("----------------------");
        Console.WriteLine("decode info:");
        foreach (var str in Jwt.Split('.'))
        {
            Console.WriteLine(str.Base64UrlDecode());
        }
    }
}
