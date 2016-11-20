using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates injection statements for properties and fields
    /// </summary>
    public interface IMemberInjectionExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request);

        /// <summary>
        /// Create member initialization statement if needed
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">initialization expression</param>
        /// <returns></returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    /// <summary>
    /// Creates injection statements for members
    /// </summary>
    public class MemberInjectionExpressionCreator : IMemberInjectionExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var returnValue = ImmutableLinkedList<ActivationStrategyDependency>.Empty;

            foreach (var memberInfo in GetMemberInjectionInfoForConfiguration(request.RequestingScope, request, configuration))
            {

            }

            return returnValue;
        }

        /// <summary>
        /// Create member initialization statement if needed
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">initialization expression</param>
        /// <returns></returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var expression = result.Expression as NewExpression;

            if (expression != null)
            {
                return CreateNewMemeberInitExpression(scope, request, activationConfiguration, result, expression);
            }

            throw new NotSupportedException("Currently only memeber injection works for New expressions");
        }

        /// <summary>
        /// Create member init expression
        /// </summary>
        /// <param name="scope">scope for configuration</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">result from instantation</param>
        /// <param name="newExpression">instantiation expression</param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateNewMemeberInitExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result, NewExpression newExpression)
        {
            var bindings = new List<MemberBinding>();

            var members = GetMemberInjectionInfoForConfiguration(scope, request, activationConfiguration);

            foreach (var memberKVP in members)
            {
                var expression = memberKVP.Value.CreateExpression;

                if (expression == null)
                {
                    var memberType = memberKVP.Key.GetMemeberType();

                    var newRequest =
                        request.NewRequest(memberType, activationConfiguration.ActivationStrategy, activationConfiguration.ActivationType, RequestType.Member, memberKVP.Value);

                    if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(memberType))
                    {
                        newRequest.SetLocateKey(memberKVP.Key.Name);
                    }

                    newRequest.IsDynamic = memberKVP.Value.IsDynamic;
                    newRequest.SetIsRequired(memberKVP.Value.IsRequired);
                    newRequest.SetFilter(memberKVP.Value.Filter);
                    newRequest.SetLocateKey(memberKVP.Value.LocateKey);

                    var memberResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                    if (memberResult == null)
                    {
                        if (memberKVP.Value.IsRequired)
                        {
                            throw new LocateException(newRequest.GetStaticInjectionContext());
                        }
                    }
                    else
                    {
                        bindings.Add(Expression.Bind(memberKVP.Key, memberResult.Expression));

                        result.AddExpressionResult(memberResult);
                    }
                }
                else
                {
                    bindings.Add(Expression.Bind(memberKVP.Key, expression));
                }
            }

            if (bindings.Count > 0)
            {
                result.Expression = Expression.MemberInit(newExpression, bindings);
            }

            return result;
        }

        /// <summary>
        /// Get dictionary of members that need to be injected
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <returns></returns>
        protected virtual Dictionary<MemberInfo, MemberInjectionInfo> GetMemberInjectionInfoForConfiguration(IInjectionScope scope,
            IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration)
        {
            var members = new Dictionary<MemberInfo, MemberInjectionInfo>();

            foreach (var memberInjectionSelector in activationConfiguration.MemberInjectionSelectors)
            {
                foreach (var memberInjectionInfo in
                    memberInjectionSelector.GetMembers(activationConfiguration.ActivationType, scope, request))
                {
                    members[memberInjectionInfo.MemberInfo] = memberInjectionInfo;
                }
            }

            return members;
        }
    }
}
