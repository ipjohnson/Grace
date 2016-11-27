using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.Dynamic.Impl
{
    public interface ILinqToDynamicMethodConverter
    {
        bool TryCreateDelegate(IActivationExpressionResult expressionContext,
            Expression finalExpression,
            out ActivationStrategyDelegate newDelegate);
    }

    public class LinqToDynamicMethodConverter : ILinqToDynamicMethodConverter
    {
        protected readonly ImplementationFactory ImplementationFactory;

        public LinqToDynamicMethodConverter(ImplementationFactory implementationFactory)
        {
            ImplementationFactory = implementationFactory;
        }

        public virtual bool TryCreateDelegate(IActivationExpressionResult expressionContext, Expression finalExpression,
            out ActivationStrategyDelegate newDelegate)
        {
            newDelegate = null;

            try
            {
                var request = new DynamicMethodGenerationRequest(expressionContext, TryGenerateIL);

                List<object> constants = new List<object>();

                if (!ImplementationFactory.Locate<IConstantExpressionCollector>().GetConstantExpressions(finalExpression, constants))
                {
                    return false;
                }

                request.Constants = constants;

                var target = ImplementationFactory.Locate<IDynamicMethodTargetCreator>().CreateMethodTarget(request);

                if (target == null)
                {
                    return false;
                }

                request.Target = target;

                var method = new DynamicMethod(string.Empty,
                    typeof(object),
                    new[]
                    {
                        target.GetType(),
                        typeof(IExportLocatorScope),
                        typeof(IDisposalScope),
                        typeof(IInjectionContext)
                    },
                    target.GetType(),
                    true);

                request.ILGenerator = method.GetILGenerator();

                if (!TryGenerateIL(request, finalExpression))
                {
                    return false;
                }

                request.ILGenerator.Emit(OpCodes.Ret);

                newDelegate = (ActivationStrategyDelegate)method.CreateDelegate(typeof(ActivationStrategyDelegate), target);

                return true;
            }
            catch (Exception exp)
            {
                // ignore exception and compile linq expression normally
            }

            return false;
        }

        protected virtual bool TryGenerateIL(DynamicMethodGenerationRequest request, Expression expression)
        {
            if (expression == null)
            {
                return true;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return ImplementationFactory.Locate<ICallExpressionGenerator>()
                        .GenerateIL(request, (MethodCallExpression)expression);

                case ExpressionType.Constant:
                    return ImplementationFactory.Locate<IConstantExpressionGenerator>()
                        .GenerateIL(request, (ConstantExpression)expression);

                case ExpressionType.MemberInit:
                    return ImplementationFactory.Locate<IMemeberInitExpressionGenerator>()
                        .GenerateIL(request, (MemberInitExpression)expression);

                case ExpressionType.New:
                    return ImplementationFactory.Locate<INewExpressionGenerator>()
                        .GenerateIL(request, (NewExpression)expression);

                case ExpressionType.NewArrayInit:
                    return ImplementationFactory.Locate<IArrayInitExpressionGenerator>()
                        .GenerateIL(request, (NewArrayExpression)expression);

                case ExpressionType.Parameter:
                    return ImplementationFactory.Locate<IParameterExpressionGenerator>()
                        .GenerateIL(request, (ParameterExpression) expression);
                    
            }

            return false;
        }
    }
}
