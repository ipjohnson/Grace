using System;
using System.Collections;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    public class DependencyInjectionContainer : RootInjectionScope, IEnumerable<object>
    {
        public DependencyInjectionContainer(Action<InjectionScopeConfiguration> configuration = null) : base(configuration)
        {
        }

        public DependencyInjectionContainer(IInjectionScopeConfiguration configuration) : base(configuration)
        {
        }

        public void Add(IConfigurationModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));

            Configure(module.Configure);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ImmutableLinkedList<object>.Empty.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
