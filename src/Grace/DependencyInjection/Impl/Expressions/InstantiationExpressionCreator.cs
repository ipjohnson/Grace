using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// implementation for creating an instantiation expression for a type
    /// </summary>
    public interface IInstantiationExpressionCreator
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

    /// <summary>
    /// Creates instantiation expressions
    /// </summary>
    public class InstantiationExpressionCreator : IInstantiationExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration,
            IActivationExpressionRequest request)
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
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            var constructor = PickConstructor(scope, activationConfiguration, request);

            scope.ScopeConfiguration.Trace?.Invoke(CreateTraceMessageForConstructor(constructor, activationConfiguration));

            var expressions = GetParameterExpressionsForConstructor(scope, activationConfiguration, request, constructor);

            return CreateConstructorExpression(request, constructor, expressions);
        }

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
        /// Method to pick the constructor to use based on how the container is configured
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="configuration">type configuration</param>
        /// <param name="request">activation request</param>
        /// <returns></returns>
        protected virtual ConstructorInfo PickConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
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

            switch (injectionScope.ScopeConfiguration.Behaviors.ConstructorSelection)
            {
                case ConstructorSelectionMethod.LeastParameters:
                    return constructors.Last();

                case ConstructorSelectionMethod.MostParameters:
                    return constructors.First();

                case ConstructorSelectionMethod.BestMatch:
                    return MatchBestConstructor(injectionScope, configuration, request, constructors);

                default:
                    throw new Exception("Unknown constrcutor selection type");
            }
        }

        /// <summary>
        /// Class used for keeping track of information about a constructor
        /// </summary>
        protected struct MatchInfo
        {
            /// <summary>
            /// how many parameters dependencies were satisfied
            /// </summary>
            public int Matched;

            /// <summary>
            /// how many parameters dependencies could not be found
            /// </summary>
            public int Missing;

            /// <summary>
            /// Constructor
            /// </summary>
            public ConstructorInfo ConstructorInfo;
        }

        /// <summary>
        /// Find best constructor to use 
        /// </summary>
        /// <returns></returns>
        protected virtual ConstructorInfo MatchBestConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, ConstructorInfo[] constructors)
        {
            ConstructorInfo returnConstructor = null;
            var matchInfos = new List<MatchInfo>();

            foreach (var constructor in constructors)
            {
                var matchInfo = new MatchInfo { ConstructorInfo = constructor };

                foreach (var parameter in constructor.GetParameters())
                {
                    object key = null;

                    if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                    {
                        key = parameter.Name;
                    }

                    if (parameter.IsOptional ||
                        parameter.ParameterType.IsGenericParameter ||
                        CanGetValueFromInfo(configuration, parameter) ||
                        injectionScope.CanLocate(parameter.ParameterType, null, key))
                    {
                        matchInfo.Matched++;
                    }
                    else
                    {
                        matchInfo.Missing++;
                    }
                }

                if (matchInfo.Missing == 0)
                {
                    returnConstructor = constructor;
                    break;
                }

                matchInfos.Add(matchInfo);
            }

            if (returnConstructor == null)
            {
                var comparer = Comparer<int>.Default;

                matchInfos.Sort((x, y) => comparer.Compare(x.Matched - x.Missing, y.Matched - y.Missing));

                returnConstructor = matchInfos.Last().ConstructorInfo;
            }

            return returnConstructor;
        }

        /// <summary>
        /// Test if the parameter was specified 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual bool CanGetValueFromInfo(TypeActivationConfiguration configuration, ParameterInfo parameter)
        {
            return false;
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

                var expression = GetParameterExpression(parameter, parameterInfo, injectionScope, configuration, request);

                returnList.Add(expression);
            }

            return returnList;
        }

        /// <summary>
        /// Find matching ConstructorParameterInfo if one exists
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected virtual ConstructorParameterInfo FindParameterInfoExpression(ParameterInfo parameter, TypeActivationConfiguration configuration)
        {
            return
                configuration.ConstructorParameters.FirstOrDefault(p => string.Compare(p.ParameterName, parameter.Name, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                                                                        (p.ParameterType == null || p.ParameterType.GetTypeInfo().IsAssignableFrom(parameter.ParameterType.GetTypeInfo()))) ??
                configuration.ConstructorParameters.FirstOrDefault(p => string.IsNullOrEmpty(p.ParameterName) &&
                                                                        p.ParameterType != null &&
                                                                        p.ParameterType.GetTypeInfo().IsAssignableFrom(parameter.ParameterType.GetTypeInfo()));
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
        protected virtual IActivationExpressionResult GetParameterExpression(ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            if (parameterInfo?.ExportFunc != null)
            {
                return CallExportFunc(configuration.ActivationStrategy, parameter, parameterInfo, injectionScope, request, configuration.ExternallyOwned);
            }

            var newRequest = request.NewRequest(parameter.ParameterType, configuration.ActivationStrategy, configuration.ActivationType, RequestType.ConstructorParameter, parameter, true);

            if (parameterInfo?.LocateWithKey != null)
            {
                newRequest.SetLocateKey(parameterInfo.LocateWithKey);
            }
            else if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
            {
                newRequest.SetLocateKey(parameter.Name);
            }

            if (parameterInfo?.DefaultValue != null)
            {
                newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = parameterInfo.DefaultValue });
            }
            else if (parameter.HasDefaultValue)
            {
                newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = parameter.DefaultValue });
            }

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

        private IActivationExpressionResult CallExportFunc(IActivationStrategy strategy, ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, IActivationExpressionRequest request, bool configurationExternallyOwned)
        {
            var exportDelegate = parameterInfo.ExportFunc as Delegate;

            if (exportDelegate == null)
            {
                throw new ArgumentException($"Parameter Info {parameterInfo.ParameterName} is not delegate",nameof(parameterInfo));
            }

            var newRequest = request.NewRequest(parameter.ParameterType, strategy, strategy.ActivationType,
                RequestType.ConstructorParameter, parameter);

            return ExpressionUtilities.CreateExpressionForDelegate(exportDelegate, ShouldTrackDisposable(configurationExternallyOwned,injectionScope,strategy), injectionScope, newRequest);
        }

        private bool ShouldTrackDisposable(bool configurationExternallyOwned, IInjectionScope scope,
            IActivationStrategy strategy)
        {
            if (configurationExternallyOwned)
            {
                return false;
            }

            return scope.ScopeConfiguration.TrackDisposableTransients ||
                   (strategy.Lifestyle != null && strategy.Lifestyle.LifestyleType != LifestyleType.Transient);
        }
    }
}
