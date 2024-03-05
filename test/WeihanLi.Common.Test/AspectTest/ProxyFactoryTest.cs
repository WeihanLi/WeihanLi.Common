using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.Event;
using WeihanLi.Common.Services;
using WeihanLi.Common.Test.EventsTest;
using Xunit;

namespace WeihanLi.Common.Test.AspectTest;

public class ProxyFactoryTest
{
    private const string NamespacePrefix = "FluentAspects.DynamicGenerated";
    private readonly IServiceProvider _serviceProvider;
    private readonly IProxyFactory _proxyFactory;

    public ProxyFactoryTest()
    {
        var services = new ServiceCollection();
        services.AddFluentAspects();
        services.AddEvents()
            ;

        services.AddTransientProxy<TestEvent>();
        services.AddTransientProxy<IEventHandler<TestEvent>, TestEventHandler>();
        services.AddTransientProxy<IUserIdProvider, EnvironmentUserIdProvider>();

        _serviceProvider = services.BuildServiceProvider();

        _proxyFactory = _serviceProvider.GetRequiredService<IProxyFactory>();
    }

    [Fact]
    public void GetProxyFromDependencyInjection()
    {
        var userIdProviderProxy = _serviceProvider.GetRequiredService<IUserIdProvider>();
        Assert.NotNull(userIdProviderProxy);
        Assert.True(userIdProviderProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

        var userId = userIdProviderProxy.GetUserId();
        Assert.NotNull(userId);

        var eventProxy = _serviceProvider.GetRequiredService<TestEvent>();
        Assert.NotNull(eventProxy);
        Assert.True(eventProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
    }

    [Fact]
    public void CreateInstanceWithoutArguments()
    {
        var userIdProviderProxy = _proxyFactory.CreateProxy<IUserIdProvider>();
        Assert.NotNull(userIdProviderProxy);
        Assert.True(userIdProviderProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

        var userIdProviderProxy2 = _proxyFactory.CreateProxy<IUserIdProvider, EnvironmentUserIdProvider>();
        Assert.NotNull(userIdProviderProxy2);
        Assert.True(userIdProviderProxy2.GetType().Namespace?.StartsWith(NamespacePrefix));

        var userId = userIdProviderProxy2.GetUserId();
        Assert.NotNull(userId);

        var eventProxy = _proxyFactory.CreateProxy<TestEvent>();
        Assert.NotNull(eventProxy);
        Assert.True(eventProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
    }

    [Fact]
    public void CreateInstanceWithArguments()
    {
        var eventPublisherProxy = _proxyFactory.CreateProxy<IEventPublisher, EventQueuePublisher>();
        Assert.NotNull(eventPublisherProxy);
        Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        eventPublisherProxy.PublishAsync(new TestEvent()).Wait();

        eventPublisherProxy = _proxyFactory.CreateProxy<EventQueuePublisher>();
        Assert.NotNull(eventPublisherProxy);
        Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        eventPublisherProxy.PublishAsync(new TestEvent()).Wait();

        var options = new OptionsWrapper<EventQueuePublisherOptions>(new EventQueuePublisherOptions());
        eventPublisherProxy = _proxyFactory.CreateProxy<EventQueuePublisher>(options);
        Assert.NotNull(eventPublisherProxy);
        Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        eventPublisherProxy.PublishAsync(new TestEvent()).Wait();
    }

    [Fact]
    public void CreateInstanceOfInterfaceWithInherit()
    {
        var eventBusProxy = _proxyFactory.CreateProxy<IEventBus>();
        Assert.NotNull(eventBusProxy);
        Assert.True(eventBusProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

        eventBusProxy.PublishAsync(new TestEvent()).Wait();
    }

    [Fact]
    public void CreateProxyInstanceOfAbstract()
    {
        var userIdProviderProxy = _proxyFactory.CreateProxy<IUserIdProvider>();
        Assert.NotNull(userIdProviderProxy);
        Assert.True(userIdProviderProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        userIdProviderProxy.GetUserId();

        var eventHandlerProxy = _proxyFactory.CreateProxy<EventHandlerBase<TestEvent>>();
        Assert.NotNull(eventHandlerProxy);
        Assert.True(eventHandlerProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        eventHandlerProxy.Handle(new TestEvent());
    }

    [Fact]
    public void CommonProxyMethodInvokeTest()
    {
        var userIdProviderProxy = _proxyFactory.CreateProxy<IUserIdProvider, EnvironmentUserIdProvider>();
        var userId = userIdProviderProxy.GetUserId();
        Assert.NotNull(userId);
        Assert.NotEmpty(userId!);
    }

    [Fact]
    public async Task GenericProxyMethodInvokeTest()
    {
        var eventQueue = _serviceProvider.GetRequiredService<IEventQueue>();
        var queueCount = (await eventQueue.GetQueuesAsync()).Count;
        Assert.Equal(0, queueCount);

        var eventPublisherProxy = _proxyFactory.CreateProxy<IEventPublisher, EventQueuePublisher>();
        await eventPublisherProxy.PublishAsync(new TestEvent());
        queueCount = (await eventQueue.GetQueuesAsync()).Count;
        Assert.Equal(1, queueCount);
    }

    [Fact]
    public async Task EventHandlerTest()
    {
        Assert.Equal(0, TestEventHandler.Count);
        var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(new TestEvent());
        Assert.Equal(1, TestEventHandler.Count);
    }

    [Fact]
    public void GenericTypeTest()
    {
        var proxyTypeFactory = _serviceProvider.GetRequiredService<IProxyTypeFactory>();
        var proxyType = proxyTypeFactory.CreateProxyType(typeof(EventHandlerBase<>));
        Assert.NotNull(proxyType);
        Assert.True(proxyType.IsGenericTypeDefinition);
        Assert.True(proxyType.IsGenericType);
        Assert.NotNull(proxyType.BaseType);

        var eventHandlerProxy = _proxyFactory.CreateProxy<EventHandlerBase<TestEvent>>();
        Assert.NotNull(eventHandlerProxy);
        Assert.True(eventHandlerProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        eventHandlerProxy.Handle(new TestEvent());
    }
}
