using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Grace.AspNetCore.MVC.Inspector
{
    /// <summary>
    /// Provides values for members that are attributed
    /// </summary>
    public class BindingSourceMetadataValueProvider : IInjectionValueProvider
    {
        private IModelMetadataProvider _modelMetadataProvider;
        private IModelBinderFactory _modelBinderFactory;

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
                    propertyInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBindingSourceMetadata) as IBinderTypeProviderMetadata;
                
                return CreateExpressionResultFromBindingAttribute(scope, 
                                                                  request,
                                                                  bindingAttribute,
                                                                  propertyInfo,
                                                                  propertyInfo.Name,
                                                                  propertyInfo.PropertyType,
                                                                  propertyInfo.GetCustomAttributes());
            }


            var fieldInfo = request.Info as PropertyInfo;

            if (fieldInfo != null)
            {
                var bindingAttribute =
                    fieldInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBindingSourceMetadata) as IBinderTypeProviderMetadata;

                return CreateExpressionResultFromBindingAttribute(scope,
                                                                  request,
                                                                  bindingAttribute,
                                                                  fieldInfo,
                                                                  fieldInfo.Name,
                                                                  fieldInfo.PropertyType,
                                                                  fieldInfo.GetCustomAttributes());
            }

            var parameterInfo = request.Info as ParameterInfo;

            if (parameterInfo != null)
            {
                var bindingAttribute =
                    parameterInfo.GetCustomAttributes(true).FirstOrDefault(a => a is IBinderTypeProviderMetadata) as IBinderTypeProviderMetadata;

                return CreateExpressionResultFromBindingAttribute(scope,
                                                                  request,
                                                                  bindingAttribute,
                                                                  parameterInfo,
                                                                  parameterInfo.Name,
                                                                  parameterInfo.ParameterType,
                                                                  parameterInfo.GetCustomAttributes(true));
            }

            return null;
        }

        private IActivationExpressionResult CreateExpressionResultFromBindingAttribute(IInjectionScope scope, 
                                                                                       IActivationExpressionRequest request, 
                                                                                       IBinderTypeProviderMetadata bindingAttribute, 
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

            if (_modelBinderFactory != null && _modelMetadataProvider != null)
            {
                var defaultValue = request.DefaultValue?.DefaultValue;

                var instance = Activator.CreateInstance(closedType, 
                                                        cacheKey, 
                                                        name, 
                                                        attributes, 
                                                        defaultValue,
                                                        request.GetStaticInjectionContext(),
                                                        _modelBinderFactory,
                                                        _modelMetadataProvider);

                var closedMethod = closedType.GetRuntimeMethod("GetValue", new[] {typeof(IExportLocatorScope)});

                var expression = Expression.Call(Expression.Constant(instance), 
                                                 closedMethod,
                                                 request.Constants.ScopeParameter);

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            return null;
        }

        public class BindingSourceHelper<T>
        {
            private readonly object _cacheKey;
            private readonly string _name;
            private readonly object _defaultValue;
            private readonly StaticInjectionContext _staticInjectionContext;
            private readonly IModelBinderFactory _binderFactory;
            private readonly BindingInfo _binding;
            private readonly ModelMetadata _metadata;

            public BindingSourceHelper(object cacheKey,
                                       string name, 
                                       IEnumerable<object> attributes, 
                                       object defaultValue,
                                       StaticInjectionContext staticInjectionContext,
                                       IModelBinderFactory binderFactory,
                                       IModelMetadataProvider metadataProvider)
            {
                _cacheKey = cacheKey;
                _name = name;
                _defaultValue = defaultValue;
                _staticInjectionContext = staticInjectionContext;
                _binderFactory = binderFactory;
                _binding = BindingInfo.GetBindingInfo(attributes);
                _metadata = metadataProvider.GetMetadataForType(typeof(T));
            }

            public T GetValue(IExportLocatorScope scope)
            {
                var activateTask = ActivateAsync(scope);

                activateTask.Wait();
                
                if (activateTask.Status == TaskStatus.RanToCompletion)
                {
                    return (T)activateTask.Result;
                }

                if (activateTask.Exception != null)
                {
                    throw new LocateException(_staticInjectionContext, 
                                              activateTask.Exception,
                                              $"Exception thrown while trying to bind to {_name}");
                }

                return default(T);
            }


            private async Task<object> ActivateAsync(IExportLocatorScope exportInjectionScope)
            {
                var binder = _binderFactory.CreateBinder(new ModelBinderFactoryContext()
                {
                    BindingInfo = _binding,
                    Metadata = _metadata,
                    CacheToken = _cacheKey,
                });

                var accessor = exportInjectionScope.Locate<IActionContextAccessor>();
                var controllerContext = new ControllerContext(accessor.ActionContext);

                var valueProvider = await CompositeValueProvider.CreateAsync(controllerContext);

                var modelBindingContext = DefaultModelBindingContext.CreateBindingContext(
                    accessor.ActionContext,
                    valueProvider,
                    _metadata,
                    _binding,
                    _name);

                var parameterModelName = _binding.BinderModelName ?? _metadata.BinderModelName;
                if (parameterModelName != null)
                {
                    // The name was set explicitly, always use that as the prefix.
                    modelBindingContext.ModelName = parameterModelName;
                }
                else if (modelBindingContext.ValueProvider.ContainsPrefix(_name))
                {
                    // We have a match for the parameter name, use that as that prefix.
                    modelBindingContext.ModelName = _name;
                }
                else
                {
                    // No match, fallback to empty string as the prefix.
                    modelBindingContext.ModelName = string.Empty;
                }

                await binder.BindModelAsync(modelBindingContext);

                if (modelBindingContext.Result.IsModelSet)
                {
                    return modelBindingContext.Result.Model;
                }

                if (_defaultValue != null)
                {
                    return (T) _defaultValue;
                }

                // if we can't match return the default value
                return default(T);
            }
        }
    }
}
