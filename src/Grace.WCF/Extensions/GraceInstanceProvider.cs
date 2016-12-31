using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Grace.DependencyInjection;
using Grace.WCF.LanguageExtensions;

namespace Grace.WCF.Extensions
{
    /// <summary>
    /// WCF instance provide
    /// </summary>
    public class GraceInstanceProvider : IInstanceProvider
    {
        private readonly IExportLocatorScope _exportLocator;
        private readonly Type _serviceType;

        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="exportLocator"></param>
        /// <param name="serviceType"></param>
        public GraceInstanceProvider(IExportLocatorScope exportLocator, Type serviceType)
        {
            _exportLocator = exportLocator;
            _serviceType = serviceType;
        }

        /// <summary>Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.</summary>
        /// <returns>A user-defined service object.</returns>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.</summary>
        /// <returns>The service object.</returns>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            object returnValue = GetRequstScope().Locate(_serviceType);

            if (returnValue == null)
            {
                throw new Exception($"Could not create new instance of  {_serviceType.FullName}");
            }

            return returnValue;
        }

        /// <summary>Called when an <see cref="T:System.ServiceModel.InstanceContext" /> object recycles a service object.</summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        private IExportLocatorScope GetRequstScope()
        {
            OperationCustomDataExtension extension = OperationContext.Current.Extensions.Find<OperationCustomDataExtension>();

            if (extension != null)
            {
                return extension.LocatorScope;
            }

            var requestScope = _exportLocator.BeginLifetimeScope();

            extension = new OperationCustomDataExtension(requestScope);

            OperationContext.Current.Extensions.Add(extension);

            return extension.LocatorScope;
        }
    }