using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                c.ExportFactory(() => new BasicService()).As<IBasicService>();
            });

            var instance = container.Locate<DependentService<IEnumerable<IBasicService>>>();
        }
    }
}
