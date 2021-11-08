using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Interface for creating linq expression to add instance to disposal scope
    /// </summary>
    public interface IDisposalScopeExpressionCreator
    {
        /// <summary>
        /// Create expression to add instance to disposal scope
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">result for instantiation</param>
        /// <returns></returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    /// <summary>
    /// Creates linq expression that add instance to disposal scope
    /// </summary>
    public class DisposalScopeExpressionCreator : IDisposalScopeExpressionCreator
    {
#if NETSTANDARD2_1
        private MethodInfo _addAsyncMethod;
        private MethodInfo _addAsyncMethodWithCleanup;
#endif
        private MethodInfo _addMethod;
        private MethodInfo _addMethodWithCleanup;

        /// <summary>
        /// Create expression to add instance to disposal scope
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">result for instantiation</param>
        /// <returns></returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var addMethod = AddMethod;
            var addMethodWithCleanup = AddMethodWithCleanup;
#if NETSTANDARD2_1
            if (activationConfiguration.ActivationType.GetTypeInfo()
                .ImplementedInterfaces.Contains(typeof(IAsyncDisposable)))
            {
                addMethod = AddAsyncMethod;
                addMethodWithCleanup = AddAsyncMethodWithCleanup;
            }
#endif
            var closedActionType = typeof(Action<>).MakeGenericType(activationConfiguration.ActivationType);

            object disposalDelegate = null;

            if (closedActionType == activationConfiguration.DisposalDelegate?.GetType())
            {
                disposalDelegate = activationConfiguration.DisposalDelegate;
            }

            MethodInfo closedGeneric;
            Expression[] parameterExpressions;

            var resultExpression = result.Expression;

            if (resultExpression.Type != activationConfiguration.ActivationType)
            {
                resultExpression = Expression.Convert(resultExpression, activationConfiguration.ActivationType);
            }

            if (disposalDelegate != null)
            {
                closedGeneric = addMethodWithCleanup.MakeGenericMethod(activationConfiguration.ActivationType);
                parameterExpressions = new[]
                    {resultExpression, Expression.Convert(Expression.Constant(disposalDelegate), closedActionType)};
            }
            else
            {
                closedGeneric = addMethod.MakeGenericMethod(activationConfiguration.ActivationType);
                parameterExpressions = new[] {resultExpression};
            }

            request.RequireDisposalScope();

            var disposalCall = Expression.Call(request.DisposalScopeExpression, closedGeneric, parameterExpressions);

            var disposalResult = request.Services.Compiler.CreateNewResult(request, disposalCall);

            disposalResult.AddExpressionResult(result);

            return disposalResult;
        }

        /// <summary>
        /// Method info for add method on IDisposalScope
        /// </summary>
        protected MethodInfo AddMethod => _addMethod ??
                                          (_addMethod = typeof(IDisposalScope).GetTypeInfo()
                                              .DeclaredMethods.First(m =>
                                                  m.Name == nameof(IDisposalScope.AddDisposable) &&
                                                  m.GetParameters().Length == 1));

        /// <summary>
        /// Method info for add method on IDisposalScope with cleanup delegate
        /// </summary>
        protected MethodInfo AddMethodWithCleanup => _addMethodWithCleanup ??
                                                     (_addMethodWithCleanup = typeof(IDisposalScope).GetTypeInfo()
                                                         .DeclaredMethods.First(m =>
                                                             m.Name == nameof(IDisposalScope.AddDisposable) &&
                                                             m.GetParameters().Length == 2));
#if NETSTANDARD2_1
        /// <summary>
        /// Method info for add async method on IDisposalScope
        /// </summary>
        protected MethodInfo AddAsyncMethod => _addAsyncMethod ??
                                               (_addAsyncMethod =
                                                   typeof(IDisposalScope).GetTypeInfo()
                                                       .DeclaredMethods.First(m =>
                                                           m.Name == nameof(IDisposalScope.AddAsyncDisposable) &&
                                                           m.GetParameters().Length == 1));

        /// <summary>
        /// Method info for add method on IDisposalScope with cleanup delegate
        /// </summary>
        protected MethodInfo AddAsyncMethodWithCleanup => _addAsyncMethodWithCleanup ??
                                                          (_addAsyncMethodWithCleanup =
                                                              typeof(IDisposalScope).GetTypeInfo()
                                                                  .DeclaredMethods.First(m =>
                                                                      m.Name ==
                                                                      nameof(IDisposalScope.AddAsyncDisposable) &&
                                                                      m.GetParameters().Length == 2));
#endif
    }
}