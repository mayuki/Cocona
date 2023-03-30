namespace Cocona.Lite.Test
{
    public class CoconaLiteInstanceActivatorTest
    {
        [Fact]
        public void GetServiceOrCreateInstance_FromService()
        {
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.ValueByType[typeof(GetServiceOrCreateInstance_FromService_Target)] = new GetServiceOrCreateInstance_FromService_Target();

            var activator = new CoconaLiteInstanceActivator();
            var obj = activator.GetServiceOrCreateInstance(serviceProvider, typeof(GetServiceOrCreateInstance_FromService_Target));
            obj.Should().Be(serviceProvider.ValueByType[typeof(GetServiceOrCreateInstance_FromService_Target)]);
        }
        class GetServiceOrCreateInstance_FromService_Target
        { }

        [Fact]
        public void GetServiceOrCreateInstance_CreateInstance()
        {
            var serviceProvider = new FakeServiceProvider();

            var activator = new CoconaLiteInstanceActivator();
            var obj = activator.GetServiceOrCreateInstance(serviceProvider, typeof(GetServiceOrCreateInstance_CreateInstance_Target));
            obj.Should().NotBeNull();
        }
        class GetServiceOrCreateInstance_CreateInstance_Target
        { }

        [Fact]
        public void CreateInstance_Insufficient()
        {
            var serviceProvider = new FakeServiceProvider();

            var activator = new CoconaLiteInstanceActivator();
            var ex = Assert.Throws<InvalidOperationException>(() => activator.CreateInstance(serviceProvider, typeof(CreateInstance_NoAdditionalParameter_Target), Array.Empty<object>()));
            ex.Message.Should().Be("Unable to resolve service for type 'System.String' while attempting to activate 'Cocona.Lite.Test.CoconaLiteInstanceActivatorTest+CreateInstance_NoAdditionalParameter_Target'");
        }

        [Fact]
        public void CreateInstance_NoAdditionalParameter()
        {
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.ValueByType[typeof(int)] = 123;
            serviceProvider.ValueByType[typeof(string)] = "Hello Konnichiwa!";

            var activator = new CoconaLiteInstanceActivator();
            var obj = activator.CreateInstance(serviceProvider, typeof(CreateInstance_NoAdditionalParameter_Target), Array.Empty<object>());
            obj.Should().BeOfType<CreateInstance_NoAdditionalParameter_Target>();
            ((CreateInstance_NoAdditionalParameter_Target)obj).Value0.Should().Be("Hello Konnichiwa!");
            ((CreateInstance_NoAdditionalParameter_Target)obj).Value1.Should().Be(123);
        }

        [Fact]
        public void CreateInstance_AdditionalParameter_NotSupported()
        {
            var serviceProvider = new FakeServiceProvider();

            var activator = new CoconaLiteInstanceActivator();
            var ex = Assert.Throws<NotSupportedException>(() => activator.CreateInstance(serviceProvider, typeof(CreateInstance_NoAdditionalParameter_Target), new object[] { 123, "Hello Konnichiwa!" }));
        }

        class CreateInstance_NoAdditionalParameter_Target
        {
            public string Value0;
            public int Value1;

            public CreateInstance_NoAdditionalParameter_Target(string value0, int value1)
            {
                Value0 = value0;
                Value1 = value1;
            }
        }

        class FakeServiceProvider : IServiceProvider
        {
            public Dictionary<Type, object> ValueByType { get; } = new Dictionary<Type, object>();

            public object GetService(Type serviceType)
            {
                return ValueByType.TryGetValue(serviceType, out var value) ? value : default;
            }
        }
    }
}
