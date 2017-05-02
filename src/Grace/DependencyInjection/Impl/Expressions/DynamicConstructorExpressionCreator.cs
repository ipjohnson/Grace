using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
            var activationDelegate = CreateActivationDelegate(scope, request, activationConfiguration);

            return CreateCallExpression(scope, request, activationConfiguration, activationDelegate);
        }

        protected virtual IActivationExpressionResult CreateCallExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, ActivationStrategyDelegate activationDelegate)
        {
            return ExpressionUtilities.CreateExpressionForDelegate(activationDelegate, activationConfiguration.ExternallyOwned,
                scope, request);
        }

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

            var foundValues = new Dictionary<string, ParameterExpression>();
            var throwAtEnd = true;

            var constructors = request.ActivationType.GetTypeInfo()
                .DeclaredConstructors
                .Where(c => c.IsPublic && !c.IsStatic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToArray();

            foreach (var constructorInfo in constructors)
            {
                var testParameters = new List<ParameterExpression>();
                var locateParameters = new List<Expression>();

                foreach (var parameter in constructorInfo.GetParameters())
                {
                    var paramKey = parameter.Name + "|" + parameter.ParameterType.FullName;

                    ParameterExpression parameterExpression;

                    if (!foundValues.TryGetValue(paramKey, out parameterExpression))
                    {
                        var canLocateInfo = CreateCanLocateStatement(parameter, scope, request, activationConfiguration);

                        // only test for required parameters with no default value
                        if (canLocateInfo != null)
                        {
                            parameterExpression = canLocateInfo.Item1;
                            variables.Add(parameterExpression);
                            testVariableExpressions.Add(canLocateInfo.Item2);
                        }
                    }

                    if (parameterExpression != null)
                    {
                        testParameters.Add(parameterExpression);
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
                testExpressions.Add(Expression.Throw(Expression.New(typeof(Exception))));
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

        private Expression CreateLocateExpression(ParameterInfo parameter, IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var closedMethod = DynamicLocateMethodInfo.MakeGenericMethod(parameter.ParameterType);

            return Expression.Call(Expression.Constant(this),
                                   closedMethod,
                                   request.Constants.ScopeParameter,
                                   request.DisposalScopeExpression,
                                   request.Constants.InjectionContextParameter,
                                   Expression.Constant(parameter.Name),
                                   Expression.Constant(true),
                                   Expression.Constant(false),
                                   Expression.Default(parameter.ParameterType));
        }

        private Tuple<ParameterExpression, Expression> CreateCanLocateStatement(ParameterInfo parameter, IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var closedMethod = DynamicCanLocateMethodInfo.MakeGenericMethod(parameter.ParameterType);

            var callExpression = Expression.Call(Expression.Constant(this),
                                                closedMethod,
                                                request.Constants.ScopeParameter,
                                                request.Constants.InjectionContextParameter,
                                                Expression.Constant(parameter.Name));

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
                return (T) value;
            }

            var currentScope = scope;

            while (currentScope != null)
            {
                value = currentScope.GetExtraData(parameterName);

                if (value is T)
                {
                    return (T) value;
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
            GetType().GetTypeInfo().GetDeclaredMethod("DynamicCanLocate");

        /// <summary>
        /// Method to use for locate
        /// </summary>
        public virtual MethodInfo DynamicLocateMethodInfo =>
            GetType().GetTypeInfo().GetDeclaredMethod("DynamicLocate");

    }
}
