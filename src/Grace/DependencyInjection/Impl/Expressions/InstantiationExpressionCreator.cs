using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IInstantiationExpressionCreator
    {
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration);
    }

    public class InstantiationExpressionCreator : IInstantiationExpressionCreator
    {
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            List<IActivationExpressionResult> expressions;

            var constructor = PickBestConstructor(scope, activationConfiguration, request, out expressions);

            return CreateConstructorExpression(request, constructor, expressions);
        }

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

        protected virtual ConstructorInfo PickBestConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, out List<IActivationExpressionResult> parameterExpressions)
        {
            var constructors = configuration.ActivationType.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic && !c.IsStatic).OrderByDescending(c => c.GetParameters().Length).ToArray();

            if (constructors.Length == 0)
            {
                throw new Exception("Could not find public constructor on type " + configuration.ActivationType.FullName);
            }

            if (constructors.Length == 1)
            {
                var constructor = constructors[0];

                return CreateActivationExpressionFromConstructor(injectionScope, configuration, request, out parameterExpressions, constructor);
            }

            var behavior = injectionScope.ScopeConfiguration.Behaviors as IExportCompilationBehaviorValues;

            switch (behavior.ConstructorSelection())
            {
                case ConstructorSelectionMethod.LeastParameters:
                    return CreateActivationExpressionFromConstructor(injectionScope, configuration, request, out parameterExpressions, constructors.Last());

                case ConstructorSelectionMethod.MostParameters:
                    return CreateActivationExpressionFromConstructor(injectionScope, configuration, request, out parameterExpressions, constructors.First());

                case ConstructorSelectionMethod.BestMatch:
                    return MatchBestConstructor(injectionScope, configuration, request, out parameterExpressions, constructors);

                default:
                    throw new Exception("Unknown constrcutor selection type");
            }
        }

        private struct MatchInfo
        {
            public int Matched;
            public int Missing;
            public ConstructorInfo ConstructorInfo;
        }

        protected virtual ConstructorInfo MatchBestConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request, out List<IActivationExpressionResult> parameterExpressions, ConstructorInfo[] constructors)
        {
            ConstructorInfo returnConstructor = null;
            List<MatchInfo> matchInfos = new List<MatchInfo>();

            foreach (var constructor in constructors)
            {
                MatchInfo matchInfo = new MatchInfo { ConstructorInfo = constructor };

                foreach (var parameter in constructor.GetParameters())
                {
                    if (injectionScope != null &&
                        injectionScope.CanLocate(parameter.ParameterType))
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

                returnConstructor = matchInfos.LastOrDefault().ConstructorInfo;
            }

            return CreateActivationExpressionFromConstructor(injectionScope, configuration, request, out parameterExpressions, returnConstructor);
        }

        protected virtual ConstructorInfo CreateActivationExpressionFromConstructor(
            IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request,
            out List<IActivationExpressionResult> parameterExpressions, ConstructorInfo constructor)
        {
            var returnList = new List<IActivationExpressionResult>();

            parameterExpressions = returnList;

            foreach (var parameter in constructor.GetParameters())
            {
                IActivationExpressionResult expression = null;
                ConstructorParameterInfo parameterInfo = null;

                if (!ReferenceEquals(configuration.ConstructorParameters, ImmutableLinkedList<ConstructorParameterInfo>.Empty))
                {
                    parameterInfo = FindParameterInfoExpression(parameter, configuration);
                }

                expression = GetParameterExpression(parameter, parameterInfo, injectionScope, configuration, request);


                returnList.Add(expression);
            }

            return constructor;
        }


        protected virtual Expression GetParameterExpressionFromInfo(ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            //var exportFunc = parameterInfo.ExportFunc as Delegate;

            //if (exportFunc != null)
            //{
            //    var getMethod = exportFunc.GetMethodInfo();

            //    request.ExpressionContext.RequireDataProvider();

            //    return Expression.Call(Expression.Constant(exportFunc.Target),
            //        getMethod,
            //        request.ExpressionContext.ScopeParameter,
            //        Expression.Constant(request.GetStaticInjectionContext()),
            //        request.ExpressionContext.ExtraDataParameter);
            //}

            return null;
            //var newRequest = new CompiledExportRequest(request, parameter.ParameterType, RequestType.ConstructorParameter, request.ActivationType, request.WrapperRequest, parameterInfo.LocateWithKey, parameter, parameterInfo.ExportStrategyFilter);

            //return objectFactoryCompiler.GetActivationExpression(injectionScope, newRequest);
        }

        protected virtual ConstructorParameterInfo FindParameterInfoExpression(ParameterInfo parameter, TypeActivationConfiguration configuration)
        {
            return
                configuration.ConstructorParameters.FirstOrDefault(p => p.ParameterName == parameter.Name &&
                                                                        p.ParameterType.GetTypeInfo().IsAssignableFrom(parameter.ParameterType.GetTypeInfo())) ??
                configuration.ConstructorParameters.FirstOrDefault(p => string.IsNullOrEmpty(p.ParameterName) &&
                                                                        p.ParameterType.GetTypeInfo().IsAssignableFrom(parameter.ParameterType.GetTypeInfo()));
        }

        protected virtual IActivationExpressionResult GetParameterExpression(ParameterInfo parameter, ConstructorParameterInfo parameterInfo, IInjectionScope injectionScope, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var expressionResult = request.Services.ExpressionBuilder.GetValueFromRequest(injectionScope, request, parameter.ParameterType, null);

            if (expressionResult != null)
            {
                return expressionResult;
            }

            var newRequest = request.NewRequest(parameter.ParameterType, configuration.ActivationStrategy, configuration.ActivationType, RequestType.ConstructorParameter, parameter, true);

            if (parameterInfo?.LocateWithKey != null)
            {
                newRequest.SetLocateKey(parameterInfo.LocateWithKey);
            }
            else if (injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector()(parameter.ParameterType))
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

            if (parameterInfo?.IsRequired != null)
            {
                newRequest.SetIsRequired(parameterInfo.IsRequired.Value);
            }

            return newRequest.Services.ExpressionBuilder.GetActivationExpression(injectionScope, newRequest);
        }
    }
}
