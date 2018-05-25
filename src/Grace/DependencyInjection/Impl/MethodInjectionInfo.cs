using System.Reflection;
using System;
using Grace.Data.Immutable;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// information for injecting a method
    /// </summary>
    public class MethodInjectionInfo
    {
        private ImmutableLinkedList<MethodParameterInfo> _parameterInfo = ImmutableLinkedList<MethodParameterInfo>.Empty;

        /// <summary>
        /// Method being injected
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Enumerable of injected method parameters information
        /// </summary>
        /// <returns>Enumerable of injected method parameters information</returns>
        public IEnumerable<MethodParameterInfo> ParameterInfos() => _parameterInfo;

        /// <summary>
        /// Add a information for injected method parameters
        /// </summary>
        /// <param name="methodParameterInfo"></param>
        public void MethodParameterInfo(MethodParameterInfo methodParameterInfo)
        {
            _parameterInfo = _parameterInfo.Add(methodParameterInfo);
        }
    }

    /// <summary>
    /// information for injected method parameters
    /// </summary>
    public class MethodParameterInfo
    {
        /// <summary>
        /// injected method parameter name
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// the locate key for injected method parameter
        /// </summary>
        public string LocateKey { get; set; }

        /// <summary>
        /// if the injected method parameter is required
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// constructor for injected method parameters
        /// </summary>
        /// <param name="parameterName">the name of the parameter</param>
        public MethodParameterInfo(string parameterName)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }
    }
}
