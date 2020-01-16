using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection.Exceptions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates a dynamic constructor expression that calls the correct constructor based on what's available in extra data
    /// </summary>
    public class DynamicConstructorExpressionCreator : ConstructorExpressionCreator
    {
        /// <summary>
        /// Create instantiation expression
        /// </summary>
        /// <param name="scope">scope the configuration is associated with</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>expression result</returns>
        public override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            request.RequireExportScope();
            
            // manipulate scope parameter rather 
            var currentScope = request.ScopeParameter;

            request.ScopeParameter = request.Constants.ScopeParameter;

            var activationDelegate = CreateActivationDelegate(scope, request, activationConfiguration);

            request.ScopeParameter = currentScope;

            return CreateCallExpression(scope, request, activationConfiguration, activationDelegate);
        }

        /// <summary>
        /// Create call expression for delegate
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="activationConfiguration"></param>
        /// <param name="activationDelegate"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateCallExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ActivationStrategyDelegate activationDelegate)
        {
            return ExpressionUtilities.CreateExpressionForDelegate(activationDelegate, activationConfiguration.ExternallyOwned,
                scope, request, activationConfiguration.ActivationStrategy);
        }

        /// <summary>
        /// Given an expression create an activation delegate
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="activationConfiguration"></param>
        /// <returns></returns>
        protected virtual ActivationStrategyDelegate CreateActivationDelegate(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var activationExpression = BuildActivationExpression(scope, request, activationConfiguration);

            return request.Services.Compiler.CompileDelegate(scope, activationExpression);
        }

        private IActivationExpressionResult BuildActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var returnLabel = Expression.Label(typeof(object));
            var variables = new List<ParameterExpression>();
            var testVariableExpressions = new List<Expression>();
            var testExpressions = new List<Expression>();

            var foundValues = new Dictionary<string, Tuple<ParameterExpression, string>>();
            var throwAtEnd = true;

            foreach (var constructorInfo in GetConstructors(activationConfiguration.ActivationType))
            {
                var testParameters = new List<ParameterExpression>();
                var locateParameters = new List<Expression>();

                foreach (var parameter in constructorInfo.GetParameters())
                {
                    var paramKey = parameter.Name + "|" + parameter.ParameterType.FullName;

                    Tuple<ParameterExpression, string> parameterExpression;

                    if (!foundValues.TryGetValue(paramKey, out parameterExpression))
                    {
                        var canLocateInfo = CreateCanLocateStatement(parameter, scope, request, activationConfiguration);

                        // only test for required parameters with no default value
                        if (canLocateInfo != null)
                        {
                            parameterExpression = new Tuple<ParameterExpression, string>(canLocateInfo.Item1, $"{parameter.ParameterType.Name} {parameter.Name}");

                            foundValues.Add(paramKey,parameterExpression);
                            variables.Add(parameterExpression.Item1);
                            testVariableExpressions.Add(canLocateInfo.Item2);
                        }
                    }

                    if (parameterExpression != null)
                    {
                        testParameters.Add(parameterExpression.Item1);
                    }

                    locateParameters.Add(CreateLocateExpression(parameter, scope, request, activationConfiguration));
                }

                var newValue = Expression.Return(returnLabel, Expression.New(constructorInfo, locateParameters));

                if (testParameters.Count > 0)
                {
                    if (testParameters.Count == 1)
                    {
                        testExpressions.Add(Expression.IfThen(Expression.IsTrue(testParameters[0]), newValue));
                    }
                    else
                    {
                        var andExpression = Expression.And(testParameters[0], testParameters[1]);

                        for (int i = 2; i < testParameters.Count; i++)
                        {
                            andExpression = Expression.And(andExpression, testParameters[i]);
                        }

                        testExpressions.Add(Expression.IfThen(andExpression, newValue));
                    }
                }
                else
                {
                    testExpressions.Add(newValue);
                    throwAtEnd = false;
                    break;
                }
            }

            if (throwAtEnd)
            {
                var constructor = typeof(LocateException).GetTypeInfo()
                    .DeclaredConstructors.Single(c => c.GetParameters().Length == 2);

                var testValues = foundValues.Values.ToArray();

                var foundArrayExpression = Expression.NewArrayInit(typeof(bool), testValues.Select(t => t.Item1));
                var labelArray = Expression.Constant(testValues.Select(t => t.Item2).ToArray());

                var createErrorStringMethod = typeof(DynamicConstructorExpressionCreator).GetTypeInfo()
                    .GetDeclaredMethod(nameof(CreateMissingErrorMessage));

                testExpressions.Add(Expression.Throw(Expression.New(constructor,
                    Expression.Constant(request.GetStaticInjectionContext()),
                    Expression.Call(createErrorStringMethod, foundArrayExpression, labelArray))));
            }

            var result =
                request.Services.Compiler.CreateNewResult(request, Expression.Label(returnLabel, Expression.Default(request.ActivationType)));

            foreach (var variable in variables)
            {
                result.AddExtraParameter(variable);
            }

            foreach (var expression in testVariableExpressions)
            {
                result.AddExtraExpression(expression);
            }

            foreach (var expression in testExpressions)
            {
                result.AddExtraExpression(expression);
            }

            return result;
        }

        private static string CreateMissingErrorMessage(bool[] found, string[] labels)
        {
            var builder = new StringBuilder("Dynamic constructor could not find the following parameters ");

            for (int i = 0; i < found.Length; i++)
            {
                if (found[i])
                {
                    continue;
                }

                builder.Append(labels[i]);

                if (i < found.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }

        private static ConstructorInfo[] GetConstructors(Type activationType)
        {
            var constructors = activationType.GetTypeInfo()
                .DeclaredConstructors
                .Where(c => c.IsPublic && !c.IsStatic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToArray();

            return constructors;
        }

        private Expression CreateLocateExpression(ParameterInfo parameter, IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var parameterInfo = FindParameterInfoExpression(parameter, activationConfiguration);

            if (parameterInfo?.ExportFunc is Delegate)
            {
                var newRequest = request.NewRequest(parameter.ParameterType, activationConfiguration.ActivationStrategy,
                    activationConfiguration.ActivationType, RequestType.ConstructorParameter, parameter, true);

                var delegateValue = (Delegate)parameterInfo.ExportFunc;

                var expressionCall = ExpressionUtilities.CreateExpressionForDelegate(delegateValue,
                    activationConfiguration.ExternallyOwned, scope, newRequest, activationConfiguration.ActivationStrategy);

                var standardParameters = delegateValue.GetMethodInfo()
                    .GetParameters()
                    .All(p =>
                    {
                        if (p.ParameterType == typeof(IExportLocatorScope))
                        {
                            return true;
                        }

                        if (p.ParameterType == typeof(IDisposalScope))
                        {
                            return true;
                        }

                        if (p.ParameterType == typeof(IInjectionContext))
                        {
                            return true;
                        }

                        return false;
                    });

                if (standardParameters)
                {
                    return expressionCall.Expression;
                }

                newRequest = request.NewRequest(parameter.ParameterType, activationConfiguration.ActivationStrategy,
                    activationConfiguration.ActivationType, RequestType.ConstructorParameter, parameter, true);

                var compiledDelegate = request.Services.Compiler.CompileDelegate(scope, expressionCall);

                expressionCall =
                    ExpressionUtilities.CreateExpressionForDelegate(compiledDelegate, false, scope, newRequest, activationConfiguration.ActivationStrategy);

                return expressionCall.Expression;
            }

            var closedMethod = DynamicLocateMethodInfo.MakeGenericMethod(parameter.ParameterType);

            return Expression.Call(Expression.Constant(this),
                closedMethod,
                request.ScopeParameter,
                request.Constants.RootDisposalScope,
                request.InjectionContextParameter,
                Expression.Constant(parameter.Name.ToLowerInvariant()),
                Expression.Constant(true),
                Expression.Constant(false),
                Expression.Default(parameter.ParameterType));
        }

        private Tuple<ParameterExpression, Expression> CreateCanLocateStatement(ParameterInfo parameter, IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var parameterInfo = FindParameterInfoExpression(parameter, activationConfiguration);

            if (parameterInfo != null &&
                (parameterInfo.DefaultValue != null || parameterInfo.ExportFunc != null))
            {
                return null;
            }

            var closedMethod = DynamicCanLocateMethodInfo.MakeGenericMethod(parameter.ParameterType);

            var callExpression = Expression.Call(Expression.Constant(this),
                                                closedMethod,
                                                request.ScopeParameter,
                                                request.InjectionContextParameter,
                                                Expression.Constant(parameter.Name.ToLowerInvariant()));

            var canVariable = Expression.Variable(typeof(bool));

            return new Tuple<ParameterExpression, Expression>(canVariable, Expression.Assign(canVariable, callExpression));
        }

        /// <summary>
        /// This method is called when there are multiple constructors
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructors"></param>
        /// <returns></returns>
        protected override ConstructorInfo PickConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration,
            IActivationExpressionRequest request, ConstructorInfo[] constructors)
        {
            return constructors.OrderByDescending(c => c.GetParameters().Length).First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public virtual bool DynamicCanLocate<T>(IExportLocatorScope scope, IInjectionContext context, string parameterName)
        {
            var value = context.GetExtraData(parameterName);

            if (value is T)
            {
                return true;
            }

            value = context.GetValueByType(typeof(T));

            if (value is T)
            {
                return true;
            }

            var currentScope = scope;

            while (currentScope != null)
            {
                if (currentScope.GetExtraData(parameterName) != null ||
                    currentScope.Values.Any(o => o is T))
                {
                    return true;
                }

                currentScope = currentScope.Parent;
            }

            return scope.GetInjectionScope().CanLocate(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="context"></param>
        /// <param name="parameterName"></param>
        /// <param name="isRequired"></param>
        /// <param name="useDefault"></param>
        /// <param name="defaultVlalue"></param>
        /// <returns></returns>
        public virtual T DynamicLocate<T>(IExportLocatorScope scope,
                                          IDisposalScope disposalScope,
                                          IInjectionContext context,
                                          string parameterName,
                                          bool isRequired,
                                          bool useDefault,
                                          T defaultVlalue)
        {
            var value = context.GetExtraData(parameterName);

            if (value is T)
            {
                return (T)value;
            }

            value = context.GetValueByType(typeof(T));

            if (value is T)
            {
                return (T)value;
            }

            value = scope.GetInjectionScope()
                .LocateFromChildScope(scope, disposalScope, typeof(T), context, null, null, true, false);

            if (value is T)
            {
                return (T)value;
            }

            var currentScope = scope;

            while (currentScope != null)
            {
                value = currentScope.GetExtraData(parameterName);

                if (value is T)
                {
                    return (T)value;
                }

                foreach (var valuePair in currentScope.KeyValuePairs)
                {
                    if (!valuePair.Key.ToString().StartsWith(UniqueStringId.Prefix) &&
                        valuePair.Value is T)
                    {
                        return (T)valuePair.Value;
                    }

                }

                currentScope = currentScope.Parent;
            }

            if (isRequired && !useDefault)
            {
                // TODO: replace with LocateException
                throw new Exception("Could not locate type");
            }

            return defaultVlalue;
        }

        /// <summary>
        /// Method to use for can locate
        /// </summary>
        public virtual MethodInfo DynamicCanLocateMethodInfo =>
            GetType().GetTypeInfo().GetDeclaredMethod(nameof(DynamicCanLocate));

        /// <summary>
        /// Method to use for locate
        /// </summary>
        public virtual MethodInfo DynamicLocateMethodInfo =>
            GetType().GetTypeInfo().GetDeclaredMethod(nameof(DynamicLocate));

    }
}
