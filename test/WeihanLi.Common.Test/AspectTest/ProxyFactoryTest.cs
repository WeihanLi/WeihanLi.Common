using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.Compressor;
using WeihanLi.Common.Event;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Services;
using WeihanLi.Common.Test.EventsTest;
using Xunit;

namespace WeihanLi.Common.Test.AspectTest
{
    public class ProxyFactoryTest
    {
        private const string NamespacePrefix = "FluentAspects.DynamicGenerated";
        private readonly IServiceProvider _serviceProvider;
        private readonly IProxyFactory _proxyFactory;

        public ProxyFactoryTest()
        {
            var services = new ServiceCollection();
            services.AddFluentAspects();
            services.AddEvents();

            services.AddTransientProxy<TestEvent>();
            services.AddTransientProxy<IUserIdProvider, EnvironmentUserIdProvider>();

            services.AddSingleton<IDataSerializer, JsonDataSerializer>();
            services.AddSingleton<IDataCompressor, NullDataCompressor>();

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
            var compressorProxy = _proxyFactory.CreateProxy<CompressDataSerializer>();
            Assert.NotNull(compressorProxy);
            Assert.True(compressorProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

            var result = compressorProxy.Serialize(new TestEvent());
            Assert.NotNull(result);

            var eventPublisherProxy = _proxyFactory.CreateProxy<IEventPublisher, EventQueuePublisher>();
            Assert.NotNull(eventPublisherProxy);
            Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

            eventPublisherProxy = _proxyFactory.CreateProxy<EventQueuePublisher>();
            Assert.NotNull(eventPublisherProxy);
            Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));

            var options = new OptionsWrapper<EventQueuePublisherOptions>(new EventQueuePublisherOptions());
            eventPublisherProxy = _proxyFactory.CreateProxy<EventQueuePublisher>(options);
            Assert.NotNull(eventPublisherProxy);
            Assert.True(eventPublisherProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        }

        [Fact]
        public void CreateInstanceOfInterfaceWithInherit()
        {
            //var eventBusProxy = _proxyFactory.CreateProxy<IEventBus>();
            //Assert.NotNull(eventBusProxy);
            //Assert.True(eventBusProxy.GetType().Namespace?.StartsWith(NamespacePrefix));
        }
    }
}
