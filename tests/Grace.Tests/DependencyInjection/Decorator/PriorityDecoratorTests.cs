using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class PriorityDecoratorTests
    {
        [Fact]
        public void DecoratorInOrderTest()
        {
            var container = new DependencyInjectionContainer{
                _ =>
                {
                    _.ExportFactory<IBasicService>(() => new BasicService {Count = 5});
                    _.ExportDecorator(typeof(DecoratorOne)).As(typeof(IBasicService));
                    _.ExportDecorator(typeof(DecoratorTwo)).As(typeof(IBasicService));
                }
            };

            var instance = container.Locate<IBasicService>();

            Assert.IsType<DecoratorTwo>(instance);
            Assert.Equal(55, instance.Count);
        }


        [Fact]
        public void DecoratorPriorityInOrderTest()
        {
            var container = new DependencyInjectionContainer{
                _ =>
                {
                    _.ExportFactory<IBasicService>(() => new BasicService {Count = 5});
                    _.ExportDecorator(typeof(DecoratorOne)).As(typeof(IBasicService));
                    _.ExportDecorator(typeof(DecoratorTwo)).As(typeof(IBasicService)).Priority(10);
                }
            };

            var instance = container.Locate<IBasicService>();

            Assert.IsType<DecoratorTwo>(instance);
            Assert.Equal(55, instance.Count);
        }


        [Fact]
        public void DecoratorPriorityOutOfOrderTest()
        {
            var container = new DependencyInjectionContainer{
                _ =>
                {
                    _.ExportFactory<IBasicService>(() => new BasicService {Count = 5});
                    _.ExportDecorator(typeof(DecoratorOne)).As(typeof(IBasicService)).Priority(10);
                    _.ExportDecorator(typeof(DecoratorTwo)).As(typeof(IBasicService));
                }
            };

            var instance = container.Locate<IBasicService>();

            Assert.IsType<DecoratorOne>(instance);
            Assert.Equal(100, instance.Count);
        }


        public class DecoratorOne : IBasicService
        {
            private IBasicService _basicService;

            public DecoratorOne(IBasicService basicService)
            {
                _basicService = basicService;
            }

            public int Count
            {
                get => _basicService.Count * 10;
                set => _basicService.Count = value;
            }

            public int TestMethod()
            {
                return _basicService.TestMethod();
            }
        }


        public class DecoratorTwo : IBasicService
        {
            private IBasicService _basicService;

            public DecoratorTwo(IBasicService basicService)
            {
                _basicService = basicService;
            }

            public int Count
            {
                get => _basicService.Count + 5;
                set => _basicService.Count = value;
            }

            public int TestMethod()
            {
                return _basicService.TestMethod();
            }
        }

    }
}
