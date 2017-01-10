using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Grace.AspNetCore.MVC.Inspector
{
    /// <summary>
    /// Provides values for members that are attributed with IBindingSourceMetadata
    /// </summary>
    public class BindingSourceMetadataValueProvider : IInjectionValueProvider
    {
        private IModelMetadataProvider _modelMetadataProvider;
        private IModelBinderFactory _modelBinderFactory;        
        private IReadOnlyList<IValueProviderFactory> _factories;

        /// <summary>
        /// Get an expression for the request, returns null if this provider doesn't support it
        /// </summary>
        /// <param name="scope">scope for request</param>
        /// <param name="request">request for expression</param>
        /// <returns>expression result</returns>
        public IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.Info == null)
            {
                return null;
            }

            var propertyInfo = request.Info as PropertyInfo;

            if (propertyInfo != null)
            {
                var bindingAttribute =
                    propertyInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBindingSourceMetadata) as IBindingSourceMetadata;

                if (bindingAttribute != null)
                {
                    return CreateExpressionResultFromBindingAttribute(scope,
                                                                  request,
                                                                  propertyInfo,
                                                                  propertyInfo.Name,
                                                                  propertyInfo.PropertyType,
                                                                  propertyInfo.GetCustomAttributes());
                }

                return null;
            }


            var fieldInfo = request.Info as PropertyInfo;

            if (fieldInfo != null)
            {
                var bindingAttribute =
                    fieldInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBindingSourceMetadata) as IBindingSourceMetadata;

                if (bindingAttribute != null)
                {
                    return CreateExpressionResultFromBindingAttribute(scope,
                        request,
                        fieldInfo,
                        fieldInfo.Name,
                        fieldInfo.PropertyType,
                        fieldInfo.GetCustomAttributes());
                }

                return null;
            }

            var parameterInfo = request.Info as ParameterInfo;

            if (parameterInfo != null)
            {
                var bindingAttribute =
                    parameterInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBindingSourceMetadata) as IBindingSourceMetadata;

                if (bindingAttribute != null)
                {
                    return CreateExpressionResultFromBindingAttribute(scope,
                        request,
                        parameterInfo,
                        parameterInfo.Name,
                        parameterInfo.ParameterType,
                        parameterInfo.GetCustomAttributes(true));
                }

                return null;
            }

            return null;
        }

        private IActivationExpressionResult CreateExpressionResultFromBindingAttribute(IInjectionScope scope,
                                                                                       IActivationExpressionRequest request,
                                                                                       object cacheKey,
                                                                                       string name,
                                                                                       Type modelType,
                                                                                       IEnumerable<object> attributes)
        {
            var closedType = typeof(BindingSourceHelper<>).MakeGenericType(modelType);


            if (_modelBinderFactory == null)
            {
                scope.TryLocate(out _modelBinderFactory);
            }

            if (_modelMetadataProvider == null)
            {
                scope.TryLocate(out _modelMetadataProvider);
            }

            if (_factories == null)
            {
                IOptions<MvcOptions> optionsAccessor;

                if (scope.TryLocate(out optionsAccessor))
                {
                    _factories = optionsAccessor.Value.ValueProviderFactories.ToArray();
                }
            }

            if (_modelBinderFactory != null && _modelMetadataProvider != null && _factories != null)
            {
                var defaultValue = request.DefaultValue?.DefaultValue;

                var instance = Activator.CreateInstance(closedType,
                                                        _factories,
                                                        name,
                                                        attributes,
                                                        defaultValue,
                                                        request.GetStaticInjectionContext());

                var closedMethod = closedType.GetRuntimeMethod("GetValue", new[] { typeof(IExportLocatorScope) });

                var expression = Expression.Call(Expression.Constant(instance),
                                                 closedMethod,
                                                 request.Constants.ScopeParameter);

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            return null;
        }

        /// <summary>
        /// Helper class for binding MVC data to dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BindingSourceHelper<T>
        {
            private readonly IReadOnlyList<IValueProviderFactory> _factories;
            private readonly string _name;
            private readonly object _defaultValue;
            private readonly StaticInjectionContext _staticInjectionContext;
            private readonly BindingInfo _binding;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="factories"></param>
            /// <param name="name"></param>
            /// <param name="attributes"></param>
            /// <param name="defaultValue"></param>
            /// <param name="staticInjectionContext"></param>
            public BindingSourceHelper(IReadOnlyList<IValueProviderFactory> factories,
                                       string name,
                                       IEnumerable<object> attributes,
                                       object defaultValue,
                                       StaticInjectionContext staticInjectionContext)
            {
                _factories = factories;
                _name = name;
                _defaultValue = defaultValue;
                _staticInjectionContext = staticInjectionContext;
                _binding = BindingInfo.GetBindingInfo(attributes);
            }

            /// <summary>
            /// Method that gets the value from MVC
            /// </summary>
            /// <param name="scope">current resolution scope</param>
            /// <returns></returns>
            public T GetValue(IExportLocatorScope scope)
            {
                var accessor = scope.Locate<IActionContextAccessor>();
                var controllerContext = new ControllerContext(accessor.ActionContext);
                var argumentBinder = scope.Locate<DefaultControllerArgumentBinder>();

                controllerContext.ValueProviderFactories = new CopyOnWriteList<IValueProviderFactory>(_factories);    
                    
                var descriptor = new ParameterDescriptor { BindingInfo = _binding, Name = _name, ParameterType = typeof(T) };

                var activateTask = argumentBinder.BindModelAsync(descriptor, controllerContext);

                activateTask.Wait();

                if (activateTask.Status == TaskStatus.RanToCompletion)
                {
                    var result = activateTask.Result;

                    if (result.IsModelSet)
                    {
                        return (T)result.Model;
                    }
                }

                if (activateTask.Exception != null)
                {
                    throw new LocateException(_staticInjectionContext,
                                              activateTask.Exception,
                                              $"Exception thrown while trying to bind to {_name}");
                }

                if (_defaultValue != null)
                {
                    return (T) _defaultValue;
                }

                return default(T);
            }
        }
    }
}
