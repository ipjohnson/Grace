using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Exceptions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IMemberInjectionExpressionCreator
    {
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    public class MemberInjectionExpressionCreator : IMemberInjectionExpressionCreator
    {
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var expression = result.Expression as NewExpression;

            if (expression != null)
            {
                return CreateNewMemeberInitExpression(scope, request, activationConfiguration, result, expression);
            }

            throw new NotSupportedException("Currently only memeber injection works for New expressions");
        }

        private IActivationExpressionResult CreateNewMemeberInitExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result, NewExpression newExpression)
        {
            var bindings = new List<MemberBinding>();
            var members = new Dictionary<MemberInfo, MemberInjectionInfo>();


            foreach (var memberInjectionSelector in activationConfiguration.MemberInjectionSelectors)
            {
                foreach (var memberInjectionInfo in memberInjectionSelector.GetMembers(activationConfiguration.ActivationType, scope, request))
                {
                    members[memberInjectionInfo.MemberInfo] = memberInjectionInfo;
                }
            }

            foreach (var memberKVP in members)
            {
                var expression = memberKVP.Value.CreateExpression;

                if (expression == null)
                {
                    var memberType = memberKVP.Key.GetMemeberType();

                    var newRequest =
                        request.NewRequest(memberType, activationConfiguration.ActivationStrategy, activationConfiguration.ActivationType, RequestType.Member, memberKVP.Value);

                    if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector()(memberType))
                    {
                        newRequest.SetLocateKey(memberKVP.Key.Name);
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
    }
}
