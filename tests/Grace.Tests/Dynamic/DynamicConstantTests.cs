using System;
using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Dynamic
{
    public class DynamicConstantTests
    {
        [Fact]
        public void DynamicMethod_1_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent1>().WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1");
            });

            var instance = container.Locate<Dependent1>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void DynamicMethod_2_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent2>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2");
            });

            var instance = container.Locate<Dependent2>();
            
            Assert.NotNull(instance);
        }

        [Fact]
        public void DynamicMethod_3_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent3>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3");
            });

            var instance = container.Locate<Dependent3>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_4_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent4>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4");
            });

            var instance = container.Locate<Dependent4>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_5_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent5>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService5");
            });

            var instance = container.Locate<Dependent5>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_6_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent6>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService5").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService6");
            });

            var instance = container.Locate<Dependent6>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_7_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent7>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService5").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService6").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService7");
            });

            var instance = container.Locate<Dependent7>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_8_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent8>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService5").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService6").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService7").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService8");
            });

            var instance = container.Locate<Dependent8>();

            Assert.NotNull(instance); ;
        }


        [Fact]
        public void DynamicMethod_9_Constant()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<Dependent9>().
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService1").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService2").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService3").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService4").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService5").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService6").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService7").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService8").
                    WithCtorParam<IBasicService>(() => new BasicService()).Named("basicService9");
            });

            var instance = container.Locate<Dependent9>();

            Assert.NotNull(instance); ;
        }

        [Fact]
        public void DynamicMethod_Primary_Constant_Test()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c => c.Export<ConstantValues>().
                                        WithCtorParam(() => "StringValue").
                                        WithCtorParam(() => 5).
                                        WithCtorParam(()=> 10.0).
                                        WithCtorParam(() => true));

            var instance = container.Locate<ConstantValues>();

            Assert.NotNull(instance);
            Assert.Equal("StringValue", instance.StringValue);
            Assert.Equal(5, instance.IntValue);
            Assert.Equal(10.0, instance.DoubleValue);
            Assert.True(instance.BoolValue);
        }

        #region DynamicConstant classes

        public class ConstantValues
        {
            public ConstantValues(string stringValue, int intValue, double doubleValue, bool boolValue)
            {
                StringValue = stringValue;
                IntValue = intValue;
                DoubleValue = doubleValue;
                BoolValue = boolValue;
            }

            public string StringValue { get; }

            public int IntValue { get; }

            public double DoubleValue { get; }

            public bool BoolValue { get; }
        }

        public class Dependent1
        {
            private readonly IBasicService _basicService1;

            public Dependent1(IBasicService basicService1)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
            }
        }

        public class Dependent2
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;

            public Dependent2(IBasicService basicService1, IBasicService basicService2)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
            }
        }

        public class Dependent3
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;

            public Dependent3(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
            }
        }


        public class Dependent4
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;

            public Dependent4(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
            }
        }

        public class Dependent5
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;
            private readonly IBasicService _basicService5;

            public Dependent5(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4, IBasicService basicService5)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
                _basicService5 = basicService5 ?? throw new ArgumentNullException(nameof(basicService5));
            }
        }

        public class Dependent6
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;
            private readonly IBasicService _basicService5;
            private readonly IBasicService _basicService6;

            public Dependent6(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4, IBasicService basicService5, IBasicService basicService6)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
                _basicService5 = basicService5 ?? throw new ArgumentNullException(nameof(basicService5));
                _basicService6 = basicService6 ?? throw new ArgumentNullException(nameof(basicService6));
            }
        }

        public class Dependent7
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;
            private readonly IBasicService _basicService5;
            private readonly IBasicService _basicService6;
            private readonly IBasicService _basicService7;

            public Dependent7(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4, IBasicService basicService5, IBasicService basicService6, IBasicService basicService7)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
                _basicService5 = basicService5 ?? throw new ArgumentNullException(nameof(basicService5));
                _basicService6 = basicService6 ?? throw new ArgumentNullException(nameof(basicService6));
                _basicService7 = basicService7 ?? throw new ArgumentNullException(nameof(basicService7));
            }
        }

        public class Dependent8
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;
            private readonly IBasicService _basicService5;
            private readonly IBasicService _basicService6;
            private readonly IBasicService _basicService7;
            private readonly IBasicService _basicService8;

            public Dependent8(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4, IBasicService basicService5, IBasicService basicService6, IBasicService basicService7, IBasicService basicService8)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
                _basicService5 = basicService5 ?? throw new ArgumentNullException(nameof(basicService5));
                _basicService6 = basicService6 ?? throw new ArgumentNullException(nameof(basicService6));
                _basicService7 = basicService7 ?? throw new ArgumentNullException(nameof(basicService7));
                _basicService8 = basicService8 ?? throw new ArgumentNullException(nameof(basicService8));
            }
        }

        public class Dependent9
        {
            private readonly IBasicService _basicService1;
            private readonly IBasicService _basicService2;
            private readonly IBasicService _basicService3;
            private readonly IBasicService _basicService4;
            private readonly IBasicService _basicService5;
            private readonly IBasicService _basicService6;
            private readonly IBasicService _basicService7;
            private readonly IBasicService _basicService8;
            private readonly IBasicService _basicService9;

            public Dependent9(IBasicService basicService1, IBasicService basicService2, IBasicService basicService3, IBasicService basicService4, IBasicService basicService5, IBasicService basicService6, IBasicService basicService7, IBasicService basicService8, IBasicService basicService9)
            {
                _basicService1 = basicService1 ?? throw new ArgumentNullException(nameof(basicService1));
                _basicService2 = basicService2 ?? throw new ArgumentNullException(nameof(basicService2));
                _basicService3 = basicService3 ?? throw new ArgumentNullException(nameof(basicService3));
                _basicService4 = basicService4 ?? throw new ArgumentNullException(nameof(basicService4));
                _basicService5 = basicService5 ?? throw new ArgumentNullException(nameof(basicService5));
                _basicService6 = basicService6 ?? throw new ArgumentNullException(nameof(basicService6));
                _basicService7 = basicService7 ?? throw new ArgumentNullException(nameof(basicService7));
                _basicService8 = basicService8 ?? throw new ArgumentNullException(nameof(basicService8));
                _basicService9 = basicService9 ?? throw new ArgumentNullException(nameof(basicService9));
            }
        }
        #endregion
    }
}
