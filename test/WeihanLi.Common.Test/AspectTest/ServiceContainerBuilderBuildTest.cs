using Microsoft.Extensions.Options;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.DependencyInjection;
using WeihanLi.Common.Event;
using WeihanLi.Common.Services;
using WeihanLi.Common.Test.EventsTest;
using Xunit;
using Xunit.Abstractions;

namespace WeihanLi.Common.Test.AspectTest;

public class ServiceContainerBuilderBuildTest
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceContainerBuilderBuildTest(ITestOutputHelper output)
    {
        var services = new ServiceContainerBuilder();
        services.AddSingleton<IIdGenerator, GuidIdGenerator>();
        services.AddSingleton(GuidIdGenerator.Instance);
        services.AddSingleton<IUserIdProvider, EnvironmentUserIdProvider>();
        services.AddSingleton<EnvironmentUserIdProvider>();

        services.AddSingleton<IEventPublisher, EventQueuePublisher>();
        services.AddSingleton<IOptions<EventQueuePublisherOptions>>(
            new OptionsWrapper<EventQueuePublisherOptions>(new EventQueuePublisherOptions()));
        services.AddSingleton<IEventQueue, EventQueueInMemory>();

        services.AddSingleton<EventHandlerBase<TestEvent>>(new DelegateEventHandler<TestEvent>(_ => { }));

        services.AddSingleton(typeof(IEventHandler<>), typeof(ServiceCollectionBuildTest.TestGenericEventHandler<>));

        _serviceProvider = services.BuildFluentAspectsContainer(options =>
        {
            options.InterceptAll()
                .With<TestOutputInterceptor>();
        });
    }

    [Fact]
    public void InterfaceTest()
    {
        // unsealed implement
        var userIdProvider = _serviceProvider.ResolveRequiredService<IUserIdProvider>();
        Assert.NotNull(userIdProvider);

        var userIdProviderType = userIdProvider.GetType();
        Assert.True(userIdProviderType.IsSealed);
        Assert.True(userIdProviderType.Assembly.IsDynamic);

        var userId = userIdProvider.GetUserId();
        Assert.NotNull(userId);

        // sealed implement
        var idGenerator = _serviceProvider.ResolveRequiredService<IIdGenerator>();
        Assert.NotNull(idGenerator);
        var idGeneratorType = idGenerator.GetType();
        Assert.True(idGeneratorType.IsSealed);
        Assert.True(idGeneratorType.Assembly.IsDynamic);

        var newId = idGenerator.NewId();
        Assert.NotNull(newId);
    }

    [Fact]
    public async Task ClassTest()
    {
        // unsealed class, will intercept virtual method
        var userIdProvider = _serviceProvider.ResolveRequiredService<EnvironmentUserIdProvider>();
        Assert.NotNull(userIdProvider);

        var userIdProviderType = userIdProvider.GetType();
        Assert.True(userIdProviderType.IsSealed);
        Assert.True(userIdProviderType.Assembly.IsDynamic);

        var userId = userIdProvider.GetUserId();
        Assert.NotNull(userId);

        // sealed class, will not be intercepted
        var idGenerator = _serviceProvider.ResolveRequiredService<GuidIdGenerator>();
        Assert.NotNull(idGenerator);
        var idGeneratorType = idGenerator.GetType();
        Assert.True(idGeneratorType.IsSealed);
        Assert.False(idGeneratorType.Assembly.IsDynamic);

        var newId = idGenerator.NewId();
        Assert.NotNull(newId);

        // unsealed service, sealed implement
        var eventHandler = _serviceProvider.ResolveRequiredService<EventHandlerBase<TestEvent>>();
        Assert.NotNull(eventHandler);
        var eventHandlerType = eventHandler.GetType();

        Assert.True(eventHandlerType.IsSealed);
        Assert.True(eventHandlerType.Assembly.IsDynamic);

        var handTask = eventHandler.Handle(new TestEvent(), new());
        Assert.NotNull(handTask);
        await handTask;
    }

    [Fact]
    public async Task GenericMethodTest()
    {
        var publisher = _serviceProvider.ResolveRequiredService<IEventPublisher>();
        Assert.NotNull(publisher);
        var publisherType = publisher.GetType();
        Assert.True(publisherType.IsSealed);
        Assert.True(publisherType.Assembly.IsDynamic);

        await publisher.PublishAsync(new TestEvent());
    }

    // not supported, will not intercept
    [Fact]
    public void OpenGenericTypeTest()
    {
        var eventHandler = _serviceProvider.ResolveRequiredService<IEventHandler<TestEvent>>();
        Assert.NotNull(eventHandler);
        var eventHandlerType = eventHandler.GetType();

        Assert.True(eventHandlerType.IsGenericType);
        //Assert.False(eventHandlerType.Assembly.IsDynamic);

        //eventHandler.Handle(new TestEvent()).Wait();
    }
}
