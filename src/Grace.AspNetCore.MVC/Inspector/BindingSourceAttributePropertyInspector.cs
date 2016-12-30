using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Grace.AspNetCore.MVC.Inspector
{
    /// <summary>
    /// Inspector class that imports properties that have MVC binding attribute (IBindingSourceMetadata)
    /// </summary>
    public class BindingSourceAttributePropertyInspector : IActivationStrategyInspector
    {
        /// <summary>
        /// Inspect the activation strategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strategy"></param>
        public void Inspect<T>(T strategy) where T : class, IActivationStrategy
        {
            var exportStrategy = strategy as ICompiledExportStrategy;

            if (exportStrategy == null || 
                exportStrategy.ActivationType.GetTypeInfo().IsAssignableFrom(typeof(Controller)))
            {
                return;
            }

            foreach (var propertyInfo in exportStrategy.ActivationType.GetRuntimeProperties())
            {
                if (!propertyInfo.CanWrite ||
                    !propertyInfo.SetMethod.IsPublic ||
                    propertyInfo.SetMethod.IsStatic)
                {
                    continue;
                }
                
                if (propertyInfo.GetCustomAttributes().Any(a => a is IBindingSourceMetadata))
                {
                    exportStrategy.MemberInjectionSelector(
                        new PropertyMemberInjectionSelector(new MemberInjectionInfo {MemberInfo = propertyInfo}));
                }
            }
        }
    }
}
