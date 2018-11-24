using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class AutomaticPropertyInjection
    {
        public class CustomClass
        {
            public IBasicService BasicService { get; }
        }

        public class AutomaticInjector : IMemberInjectionSelector
        {
            /// <summary>
            /// Get a list of member injection info for a specific type
            /// </summary>
            /// <param name="type">type being activated</param>
            /// <param name="injectionScope">injection scope</param>
            /// <param name="request">request</param>
            /// <returns>members being injected</returns>
            public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
            {
                var autoProperties = new List<PropertyInfo>();

                foreach (var propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.CanRead && propertyInfo.SetMethod == null)
                    {
                        autoProperties.Add(propertyInfo);
                    }
                }

                foreach (var field in type.GetTypeInfo().DeclaredFields)
                {
                    if (field.IsPrivate)
                    {
                        var property = autoProperties.Find(p => field.Name.Contains(p.Name));

                        if (property != null)
                        {
                            yield return new MemberInjectionInfo { MemberInfo = field };
                        }
                    }
                }
            }

            /// <summary>
            /// Get Methods to inject
            /// </summary>
            /// <param name="type">type being activated</param>
            /// <param name="injectionScope">injection scope</param>
            /// <param name="request">request</param>
            /// <returns>methods being injected</returns>
            public IEnumerable<MethodInjectionInfo> GetMethods(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
            {
                yield break;
            }
        }

#if NETCOREAPP
        [Fact]
        public void InjectBasicServiceIntoAutoProperty()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(api =>
            {
                api.Export<BasicService>().As<IBasicService>();
                api.AddMemberInjectionSelector(new AutomaticInjector());
            });

            var instance = container.Locate<CustomClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
        }
#endif
    }
}
