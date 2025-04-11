using WildCoco.Lite;

namespace WildCoco.Lite.Test;

public class WildCocoLiteServiceCollectionTest
{
    [Fact]
    public void AddTransient_Factory()
    {
        var services = new WildCocoLiteServiceCollection();
        services.AddTransient<IMyService>(_ => new MyService());

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    [Fact]
    public void AddSingleton_Factory()
    {
        var services = new WildCocoLiteServiceCollection();
        services.AddSingleton<IMyService>(_ => new MyService());

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    [Fact]
    public void AddTransient_Type()
    {
        var services = new WildCocoLiteServiceCollection();
        services.AddTransient<IMyService, MyService>();

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    [Fact]
    public void AddSingleton_Type()
    {
        var services = new WildCocoLiteServiceCollection();
        services.AddSingleton<IMyService, MyService>();

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    [Fact]
    public void TryAddSingleton()
    {
        var services = new WildCocoLiteServiceCollection();
        services.TryAddSingleton<IMyService, MyService>();
        services.TryAddSingleton<IMyService, MyService2>();

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    [Fact]
    public void TryAddTransient()
    {
        var services = new WildCocoLiteServiceCollection();
        services.TryAddTransient<IMyService, MyService>();
        services.TryAddTransient<IMyService, MyService2>();

        services.Should().HaveCount(1);
        services.Should().Contain(x => x.ServiceType == typeof(IMyService) && x.Factory != null);
    }

    public interface IMyService
    {
    }

    public class MyService : IMyService
    {
    }
    public class MyService2 : IMyService
    {
    }
}
