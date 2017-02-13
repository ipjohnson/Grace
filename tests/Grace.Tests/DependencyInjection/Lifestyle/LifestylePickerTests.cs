using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class LifestylePickerTests
    {
        [Fact]
        public void LifestylePicker_Custom()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.Custom(new SingletonLifestyle()));

            var instance = container.Locate<IBasicService>();
            Assert.Same(instance, container.Locate<IBasicService>());
        }

        [Fact]
        public void LifestylePicker_NamedScope_Null_Throws()
        {
            var lifestylePicker = new LifestylePicker<IBasicService>(new BasicService(), lifestyle => { });

            Assert.Throws<ArgumentNullException>(() => lifestylePicker.SingletonPerNamedScope(null));
        }
        
        [Fact]
        public void LifestylePicker_PerAncestor_Null_Throws()
        {
            var lifestylePicker = new LifestylePicker<IBasicService>(new BasicService(), lifestyle => { });

            Assert.Throws<ArgumentNullException>(() => lifestylePicker.SingletonPerAncestor(null));
        }
    }
}
