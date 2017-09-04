using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class TimedConstructorSelectionMethod : BestMatchConstructorExpressionCreator
    {
        private MethodInfo _preMethodInfo;
        private MethodInfo _postMethodInfo;

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
            var expression = base.CreateExpression(scope, request, activationConfiguration);

            var timedCreateExpression = CreateTimedCreateExpression(scope, request, activationConfiguration, expression);

            var createDelegate = request.Services.Compiler.CompileDelegate(scope, timedCreateExpression);

            return ExpressionUtilities.CreateExpressionForDelegate(createDelegate, false, scope, request);
        }

        private IActivationExpressionResult CreateTimedCreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult expression)
        {
            var result = request.Services.Compiler.CreateNewResult(request);
            
            var returnVar = Expression.Variable(typeof(object));
            var staticContext = request.GetStaticInjectionContext();

            var callExpression = Expression.Call(Expression.Constant(this),
                                                    PreBuildUpMethodInfo,
                                                    Expression.Constant(staticContext),
                                                    request.Constants.ScopeParameter,
                                                    request.InjectionContextParameter);

            result.AddExtraParameter(returnVar);
            result.AddExtraExpression(Expression.Assign(returnVar, callExpression));

            result.AddExtraExpression(
                Expression.IfThen(Expression.Equal(returnVar,
                    Expression.Constant(null, typeof(object))),
                        Expression.Assign(returnVar, expression.Expression)));

            result.Expression = Expression.Call(Expression.Constant(this),
                PostBuildUpMethodInfo,
                returnVar,
                Expression.Constant(staticContext),
                request.ScopeParameter,
                request.InjectionContextParameter);

            return result;
        }
        
        private MethodInfo PreBuildUpMethodInfo =>
            _preMethodInfo ?? (_preMethodInfo = GetType().GetMethod("PreBuildUp"));

        private MethodInfo PostBuildUpMethodInfo =>
            _postMethodInfo ?? (_postMethodInfo = GetType().GetMethod("PostBuildUp"));

        public virtual object PreBuildUp(StaticInjectionContext staticContext, IExportLocatorScope scope,
            IInjectionContext injectionContext)
        {
            var timer = new Stopwatch();

            injectionContext.SetExtraData(staticContext.TargetInfo.UniqueId, timer);

            timer.Start();

            return null;
        }

        public virtual object PostBuildUp(object value, StaticInjectionContext staticContext, IExportLocatorScope scope,
            IInjectionContext injectionContext)
        {
            var timer = (Stopwatch)injectionContext.GetExtraData(staticContext.TargetInfo.UniqueId);

            timer.Stop();
            
            return value;
        }
    }
}
