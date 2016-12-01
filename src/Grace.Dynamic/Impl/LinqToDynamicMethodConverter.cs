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
    /// <summary>
    /// interface to convert linq expression to IL using dynamic method
    /// </summary>
    public interface ILinqToDynamicMethodConverter
    {
        /// <summary>
        /// try to create delegate using IL generation
        /// </summary>
        /// <param name="expressionContext">expression context</param>
        /// <param name="finalExpression">final expression to convert</param>
        /// <param name="newDelegate">created delegate</param>
        /// <returns>true if delegate was created</returns>
        bool TryCreateDelegate(IActivationExpressionResult expressionContext,
            Expression finalExpression,
            out ActivationStrategyDelegate newDelegate);
    }

    /// <summary>
    /// class to convert linq expression to IL using dynamic method
    /// </summary>
    public class LinqToDynamicMethodConverter : ILinqToDynamicMethodConverter
    {
        /// <summary>
        /// Implementation factory
        /// </summary>
        protected readonly ImplementationFactory ImplementationFactory;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="implementationFactory"></param>
        public LinqToDynamicMethodConverter(ImplementationFactory implementationFactory)
        {
            ImplementationFactory = implementationFactory;
        }

        /// <summary>
        /// try to create delegate using IL generation
        /// </summary>
        /// <param name="expressionContext">expression context</param>
        /// <param name="finalExpression">final expression to convert</param>
        /// <param name="newDelegate">created delegate</param>
        /// <returns>true if delegate was created</returns>
        public virtual bool TryCreateDelegate(IActivationExpressionResult expressionContext, Expression finalExpression,
            out ActivationStrategyDelegate newDelegate)
        {
            newDelegate = null;

            try
            {
                var request = new DynamicMethodGenerationRequest(expressionContext, TryGenerateIL);

                var constants = new List<object>();

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
            catch (Exception)
            {
                // ignore exception and compile linq expression normally
            }

            return false;
        }

        /// <summary>
        /// Try to create IL from linq expression, this is called recursively as the expression is walked
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression to convert</param>
        /// <returns></returns>
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
