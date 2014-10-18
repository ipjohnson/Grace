// Author: Prasanna V. Loganathar
// Created: 6:56 AM 12-10-2014

using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Microsoft.Practices.ServiceLocation;

namespace Grace.CommonServiceLocator
{
    /// <summary>
    ///     Grace implementation of the Microsoft CommonServiceLocator.
    /// </summary>
    public class GraceServiceLocator : ServiceLocatorImplBase
    {
        private readonly DependencyInjectionContainer container;

        public GraceServiceLocator(DependencyInjectionContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public DependencyInjectionContainer GetContainer()
        {
            return container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            return key != null ? container.Locate(serviceType, withKey: key) : container.Locate(serviceType);
        }


        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            return container.LocateAll(serviceType).AsEnumerable();
        }
    }
}