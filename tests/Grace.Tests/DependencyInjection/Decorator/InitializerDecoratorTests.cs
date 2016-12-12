namespace Grace.Tests.DependencyInjection.Decorator
{
    public class InitializerDecoratorTests
    {
        // commenting out for the moment as I don't like the implementation
        //[Fact]
        //public void Initialize_Basic_Service()
        //{
        //    var container = new DependencyInjectionContainer();

        //    container.Configure(c =>
        //    {
        //        c.ExportInitialize<BasicService>(service => service.Count = 5);
        //        c.Export<BasicService>().As<IBasicService>();
        //    });

        //    var instance = container.Locate<BasicService>();

        //    Assert.NotNull(instance);
        //    Assert.Equal(5, instance.Count);
        //}
    }
}
