using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Grace.Data.Immutable;
using System.Linq.Expressions;
using System.Linq;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstructorExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration,
            IActivationExpressionRequest request);

        /// <summary>
        /// Create instantiation expression
        /// </summary>
        /// <param name="scope">scope the configuration is associated with</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>expression result</returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration);
    }

    public abstract class ConstructorExpressionCreator : IConstructorExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public virtual IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var constructor = PickConstructor(request.RequestingScope, configuration, request);

            return GetDependenciesForConstructor(configuration, request, constructor);
        }

        /// <summary>
        /// Create instantiation expression
        /// </summary>
        /// <param name="scope">scope the configuration is associated with</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>expression result</returns>
        public virtual IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            var constructor = PickConstructor(scope, activationConfiguration, request);

            scope.ScopeConfiguration.Trace?.Invoke(CreateTraceMessageForConstructor(constructor, activationConfiguration));

            var expressions = GetParameterExpressionsForConstructor(scope, activationConfiguration, request, constructor);

            return CreateConstructorExpression(request, constructor, expressions);
        }

        /// <summary>
        /// Get a list of expressions for the parameters in a constructor
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="configuration">configuration for strategy</param>
        /// <param name="request">request</param>
        /// <param name="constructor">constructor</param>
        /// <returns></returns>
        protected virtual List<IActivationExpressionResult> GetParameterExpressionsForConstructor(
            IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request,
            ConstructorInfo constructor)
        {
            var returnList = new List<IActivationExpressionResult>();

            foreach (var parameter in constructor.GetParameters())
            {
                ConstructorParameterInfo parameterInfo = null;

                if (!ReferenceEquals(configuration.ConstructorParameters, ImmutableLinkedList<ConstructorParameterInfo>.Empty))
                {
                    parameterInfo = FindParameterInfoExpression(parameter, configuration);
                }

                if (parameterInfo == null &&
                    injectionScope.ScopeConfiguration.Behaviors.ProcessImportAttributeForParameters)
                {
                    parameterInfo = ProcessImportAttributes(parameter);
                }

                var expression = GetParameterExpression(parameter, parameterInfo, injectionScope, configuration, request, out var newRequest);

                expression = OverrideExpression(injectionScope, configuration, newRequest, constructor, expression);

                returnList.Add(expression);
            }

            return returnList;
        }

        private ConstructorParameterInfo ProcessImportAttributes(ParameterInfo parameter)
        {
            var importAttribute = (IImportAttribute)parameter.GetCustomAttributes()?.FirstOrDefault(a => a is IImportAttribute);

            if (importAttribute != null)
            {
                var info = importAttribute.ProvideImportInfo(parameter.ParameterType, parameter.Name);

                if (info != null)
                {
                    return new ConstructorParameterInfo(null)
                    {
                        LocateWithKey = info.ImportKey,
                        DefaultValue = info.DefaultValue,
                        EnumerableComparer = info.Comparer,
                        ExportStrategyFilter = info.ExportStrategyFilter,
                        IsRequired = info.IsRequired
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Allow constructor selector to override method
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructor"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult OverrideExpression(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo constructor, IActivationExpressionResult expression)
        {
            return expression;
        }

        /// <summary>
        /// Find matching ConstructorParameterInfo if one exists
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected virtual ConstructorParameterInfo FindParameterInfoExpression(ParameterInfo parameter, TypeActivationConfiguration configuration)
        {
            var parameterInfo = parameter.ParameterType.GetTypeInfo();

            var matchedConstructor = configuration.ConstructorParameters.FirstOrDefault(
                p => string.Compare(p.ParameterName, parameter.Name, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                     (p.ParameterType == null ||
                      p.ParameterType.GetTypeInfo().IsAssignableFrom(parameterInfo) ||
                      parameterInfo.IsAssignableFrom(p.ParameterType.GetTypeInfo())));

            if (matchedConstructor != null)
            {
                return matchedConstructor;
            }

            return configuration.ConstructorParameters.FirstOrDefault(p => string.IsNullOrEmpty(p.ParameterName) &&
                                                                        p.ParameterType != null &&
                                                                        (parameterInfo.IsAssignableFrom(p.ParameterType.GetTypeInfo()) ||
                                                                        p.ParameterType.GetTypeInfo().IsAssignableFrom(parameterInfo)));
        }

        /// <summary>
        /// Get expression for parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetParameterExpression(ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, out IActivationExpressionRequest newRequest)
        {
            if (parameterInfo?.ExportFunc != null)
            {
                newRequest = null;

                return CallExportFunc(configuration.ActivationStrategy, parameter, parameterInfo, injectionScope, request, configuration.ExternallyOwned);
            }

            newRequest = request.NewRequest(parameterInfo?.UseType ?? parameter.ParameterType,
                configuration.ActivationStrategy,
                configuration.ActivationType,
                RequestType.ConstructorParameter,
                parameter,
                true,
                true);

            if (parameterInfo?.LocateWithKey != null)
            {
                newRequest.SetLocateKey(parameterInfo.LocateWithKey);
            }
            else if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
            {
                newRequest.SetLocateKey(parameter.Name);
            }

            SetDefaultValue(parameter, parameterInfo, newRequest);

            if (parameterInfo != null)
            {
                newRequest.IsDynamic = parameterInfo.IsDynamic;

                newRequest.SetIsRequired(parameterInfo.IsRequired.GetValueOrDefault(!parameter.IsOptional));

                newRequest.SetFilter(parameterInfo.ExportStrategyFilter);

                newRequest.SetEnumerableComparer(parameterInfo.EnumerableComparer);
            }
            else
            {
                newRequest.SetIsRequired(!parameter.IsOptional);
            }

            return newRequest.Services.ExpressionBuilder.GetActivationExpression(injectionScope, newRequest);
        }

        private static void SetDefaultValue(ParameterInfo parameter, ConstructorParameterInfo parameterInfo,
            IActivationExpressionRequest newRequest)
        {
            try
            {
                if (parameterInfo?.DefaultValue != null)
                {
                    newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = parameterInfo.DefaultValue });
                }
                else if (parameter.HasDefaultValue)
                {
                    var defaultValue = parameter.DefaultValue;

                    if (defaultValue == null && 
                        parameter.ParameterType.GetTypeInfo().IsValueType)
                    {
                        defaultValue = Activator.CreateInstance(parameter.ParameterType);
                    }

                    newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = defaultValue});
                }
            }
            catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
            {
                newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = default(DateTime) });
            }
        }

        /// <summary>
        /// Create expression to call func
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="parameter"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="injectionScope"></param>
        /// <param name="request"></param>
        /// <param name="configurationExternallyOwned"></param>
        /// <returns></returns>
        protected IActivationExpressionResult CallExportFunc(IActivationStrategy strategy, ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, IActivationExpressionRequest request, bool configurationExternallyOwned)
        {
            var exportDelegate = parameterInfo.ExportFunc as Delegate;

            if (exportDelegate == null)
            {
                throw new ArgumentException($"Parameter Info {parameterInfo.ParameterName} is not delegate", nameof(parameterInfo));
            }

            var newRequest = request.NewRequest(parameter.ParameterType, strategy, strategy.ActivationType,
                RequestType.ConstructorParameter, parameter, false, true);

            return ExpressionUtilities.CreateExpressionForDelegate(exportDelegate, ShouldTrackDisposable(configurationExternallyOwned, injectionScope, strategy), injectionScope, newRequest, strategy);
        }

        /// <summary>
        /// Should the export be tracked for disposal
        /// </summary>
        /// <param name="configurationExternallyOwned"></param>
        /// <param name="scope"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        protected bool ShouldTrackDisposable(bool configurationExternallyOwned, IInjectionScope scope,
            IActivationStrategy strategy)
        {
            if (configurationExternallyOwned)
            {
                return false;
            }

            return scope.ScopeConfiguration.TrackDisposableTransients ||
                   (strategy.Lifestyle != null && strategy.Lifestyle.LifestyleType != LifestyleType.Transient);
        }

        /// <summary>
        /// This method is used to pick a constructor from a type
        /// </summary>
        /// <param name="requestingScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual ConstructorInfo PickConstructor(IInjectionScope requestingScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            if (configuration.SelectedConstructor != null)
            {
                return configuration.SelectedConstructor;
            }

            var constructors = configuration.ActivationType.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic && !c.IsStatic).OrderByDescending(c => c.GetParameters().Length).ToArray();

            if (constructors.Length == 0)
            {
                throw new Exception("Could not find public constructor on type " + configuration.ActivationType.FullName);
            }

            if (constructors.Length == 1)
            {
                return constructors[0];
            }

            return PickConstructor(requestingScope, configuration, request, constructors);
        }

        /// <summary>
        /// This method is called when there are multiple constructors
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructors"></param>
        /// <returns></returns>
        protected abstract ConstructorInfo PickConstructor(IInjectionScope injectionScope,
            TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo[] constructors);

        /// <summary>
        /// Creates trace message for specific constructor
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="activationConfiguration"></param>
        /// <returns></returns>
        protected virtual string CreateTraceMessageForConstructor(ConstructorInfo constructor, TypeActivationConfiguration activationConfiguration)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(activationConfiguration.ActivationType.Name);
            builder.Append('(');

            var parameters = constructor.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }

                builder.Append(parameters[i].ParameterType.Name);
                builder.Append(' ');
                builder.Append(parameters[i].Name);
            }

            builder.Append(')');

            return "Using " + builder;
        }

        /// <summary>
        /// Get a list of dependencies for a constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ActivationStrategyDependency> GetDependenciesForConstructor(TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo constructor)
        {
            var dependencies = ImmutableLinkedList<ActivationStrategyDependency>.Empty;
            var injectionScope = request.RequestingScope;

            foreach (var parameter in constructor.GetParameters())
            {
                object key = null;

                if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                {
                    key = parameter.Name;
                }

                var dependencySatisified = parameter.IsOptional ||
                                           parameter.ParameterType.IsGenericParameter ||
                                           CanGetValueFromInfo(configuration, parameter) ||
                                           injectionScope.CanLocate(parameter.ParameterType, null, key);

                var dependency = new ActivationStrategyDependency(DependencyType.ConstructorParameter,
                    configuration.ActivationStrategy,
                    parameter,
                    parameter.ParameterType,
                    parameter.Name,
                    false,
                    false,
                    dependencySatisified);

                dependencies = dependencies.Add(dependency);
            }

            return dependencies;
        }

        /// <summary>
        /// Creates an expression to call a constructor given a ConstructorInfo and a list of parameters
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="constructor">constructor</param>
        /// <param name="expressions">parameters</param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateConstructorExpression(IActivationExpressionRequest request, ConstructorInfo constructor, List<IActivationExpressionResult> expressions)
        {
            var newExpression = Expression.New(constructor, expressions.Select(e => e.Expression));

            var result = request.Services.Compiler.CreateNewResult(request, newExpression);

            foreach (var expressionResult in expressions)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;
        }

        /// <summary>
        /// Test if the parameter was specified 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual bool CanGetValueFromInfo(TypeActivationConfiguration configuration, ParameterInfo parameter)
        {
            var matchedParameter = FindParameterInfoExpression(parameter, configuration);

            if (matchedParameter == null)
            {
                return false;
            }

            return matchedParameter.ExportFunc != null ||
                  !matchedParameter.IsRequired.GetValueOrDefault(true) ||
                   matchedParameter.UseType != null ||
                   matchedParameter.DefaultValue != null;
        }
    }
}
