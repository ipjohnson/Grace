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

            foreach (var kvp in GetMemberInjectionInfoForConfiguration(request.RequestingScope, request, configuration))
            {
                var memberType = kvp.Key.GetMemberType();
                object key = null;

                if (request.RequestingScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(memberType))
                {
                    key = kvp.Key.Name;
                }

                var found = memberType.IsGenericParameter ||
                            request.RequestingScope.CanLocate(memberType, key: key);

                returnValue =
                    returnValue.Add(new ActivationStrategyDependency(kvp.Key is PropertyInfo ? DependencyType.Property : DependencyType.Field,
                                                                     configuration.ActivationStrategy,
                                                                     kvp.Key,
                                                                     kvp.Key.GetMemberType(),
                                                                     kvp.Key.Name,
                                                                     false,
                                                                     false,
                                                                     found));
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

            return CreateMemberInjectExpressions(scope, request, activationConfiguration, result);
        }

        /// <summary>
        /// Creates member injection statements
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="activationConfiguration"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateMemberInjectExpressions(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var members = GetMemberInjectionInfoForConfiguration(scope, request, activationConfiguration);

            if (members.Count == 0)
            {
                return result;
            }

            ParameterExpression variable = result.Expression as ParameterExpression;

            var newResult = request.Services.Compiler.CreateNewResult(request);

            if (variable == null)
            {
                variable = Expression.Variable(result.Expression.Type);

                newResult.AddExpressionResult(result);

                newResult.AddExtraParameter(variable);
                newResult.AddExtraExpression(Expression.Assign(variable, result.Expression));
            }

            foreach (var memberKVP in members)
            {
                var expression = memberKVP.Value.CreateExpression;

                if (expression == null)
                {
                    var memberType = memberKVP.Key.GetMemberType();

                    var newRequest =
                        request.NewRequest(memberType, activationConfiguration.ActivationStrategy,
                            activationConfiguration.ActivationType, RequestType.Member, memberKVP.Key, false, true);

                    if (memberKVP.Value.LocateKey == null &&
                        scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(memberType))
                    {
                        newRequest.SetLocateKey(memberKVP.Key.Name);
                    }
                    else
                    {
                        newRequest.SetLocateKey(memberKVP.Value.LocateKey);
                    }

                    newRequest.IsDynamic = memberKVP.Value.IsDynamic;
                    newRequest.SetIsRequired(memberKVP.Value.IsRequired);
                    newRequest.SetFilter(memberKVP.Value.Filter);

                    if (memberKVP.Value.DefaultValue != null)
                    {
                        newRequest.SetDefaultValue(
                            new DefaultValueInformation { DefaultValue = memberKVP.Value.DefaultValue });
                    }

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
                        Expression memberExpression;

                        if (memberKVP.Key is FieldInfo)
                        {
                            memberExpression = Expression.Field(variable, memberKVP.Key as FieldInfo);
                        }
                        else if (memberKVP.Key is PropertyInfo)
                        {
                            memberExpression = Expression.Property(variable, (PropertyInfo)memberKVP.Key);
                        }
                        else
                        {
                            throw new LocateException(request.GetStaticInjectionContext(), $"{memberKVP.Key.GetType().Name} member type not supported");
                        }

                        newResult.AddExpressionResult(memberResult);

                        newResult.AddExtraExpression(Expression.Assign(memberExpression, memberResult.Expression));
                    }
                }
                else
                {
                    Expression memberExpression;

                    if (memberKVP.Key is FieldInfo)
                    {
                        memberExpression = Expression.Field(variable, memberKVP.Key as FieldInfo);
                    }
                    else if (memberKVP.Key is PropertyInfo)
                    {
                        memberExpression = Expression.Property(variable, (PropertyInfo)memberKVP.Key);
                    }
                    else
                    {
                        throw new LocateException(request.GetStaticInjectionContext(), $"{memberKVP.Key.GetType().Name} member type not supported");
                    }
                    
                    newResult.AddExtraExpression(Expression.Assign(memberExpression, expression));
                }
            }

            newResult.Expression = variable;

            return newResult;
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
                    var memberType = memberKVP.Key.GetMemberType();

                    var newRequest =
                        request.NewRequest(memberType, activationConfiguration.ActivationStrategy, activationConfiguration.ActivationType, RequestType.Member, memberKVP.Key, false, true);

                    if (memberKVP.Value.LocateKey == null &&
                        scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(memberType))
                    {
                        newRequest.SetLocateKey(memberKVP.Key.Name);
                    }
                    else
                    {
                        newRequest.SetLocateKey(memberKVP.Value.LocateKey);
                    }

                    newRequest.IsDynamic = memberKVP.Value.IsDynamic;
                    newRequest.SetIsRequired(memberKVP.Value.IsRequired);
                    newRequest.SetFilter(memberKVP.Value.Filter);

                    if (memberKVP.Value.DefaultValue != null)
                    {
                        newRequest.SetDefaultValue(new DefaultValueInformation { DefaultValue = memberKVP.Value.DefaultValue });
                    }

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

            var currentScope = scope;

            while (currentScope != null)
            {
                ProcessMemberSelectors(scope, request, activationConfiguration.ActivationType, currentScope.MemberInjectionSelectors, members);

                currentScope = currentScope.Parent as IInjectionScope;
            }

            ProcessMemberSelectors(scope, request, activationConfiguration.ActivationType, activationConfiguration.MemberInjectionSelectors, members);

            return members;
        }

        private static void ProcessMemberSelectors(IInjectionScope scope, IActivationExpressionRequest request,
            Type activationType, IEnumerable<IMemberInjectionSelector> selectors,
            Dictionary<MemberInfo, MemberInjectionInfo> members)
        {
            foreach (var memberInjectionSelector in selectors)
            {
                foreach (var memberInjectionInfo in
                    memberInjectionSelector.GetPropertiesAndFields(activationType, scope, request))
                {
                    members[memberInjectionInfo.MemberInfo] = memberInjectionInfo;
                }
            }
        }
    }
}
