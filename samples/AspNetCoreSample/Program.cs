using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.Event;

namespace AspNetCoreSample;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            })
            //.UseServiceProviderFactory()
            .UseFluentAspectsServiceProviderFactory(options =>
            {
                options
                    .InterceptType<IEventPublisher>()
                    .With<EventPublishLogInterceptor>();

                options.InterceptType<IEventHandler>()
                    .With<EventHandleLogInterceptor>();
            }, builder =>
            {
                    //builder.UseCastleProxy();
                })
            .Build();
        //var fields = host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        //    ;
        host.Run();
    }
}
