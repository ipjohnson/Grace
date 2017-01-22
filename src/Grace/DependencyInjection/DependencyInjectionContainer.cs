using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Dependancy injection container, this is the main class to instantiate 
    /// </summary>
    public class DependencyInjectionContainer : InjectionScope, IEnumerable<object>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">provide method to configure container behavior</param>
        public DependencyInjectionContainer(Action<InjectionScopeConfiguration> configuration = null) : base(configuration)
        {
        }

        /// <summary>
        /// Constructor requiring an injection scope configuration object be provided
        /// </summary>
        /// <param name="configuration">configuration object</param>
        public DependencyInjectionContainer(IInjectionScopeConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Add configuration module to container
        /// </summary>
        /// <param name="module"></param>
        public void Add(IConfigurationModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));

            Configure(module.Configure);
        }

        /// <summary>
        /// Add registration delegate to container
        /// </summary>
        /// <param name="registrationAction"></param>
        public void Add(Action<IExportRegistrationBlock> registrationAction)
        {
            if (registrationAction == null) throw new ArgumentNullException(nameof(registrationAction));

            Configure(registrationAction);
        }

        /// <summary>
        /// This is here to allow adding configuration modules through object initialization. Always returns empty
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerator<object> GetEnumerator()
        {
            return ImmutableLinkedList<object>.Empty.GetEnumerator();
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
