using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Grace.Dynamic.Console
{
    public class DependentService<T>
    {
        public DependentService(T value)
        {
            TValue = value;
        }

        public T TValue { get; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration());

            container.Configure(c =>
            {
                c.ExportFactory(() => new BasicService()).As<IBasicService>().Lifestyle.Singleton();
            });

            var instance = container.Locate<DependentService<IEnumerable<IBasicService>>>();
        }
    }
}
