using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// interface for collecting information about constants
    /// </summary>
    public interface IConstantExpressionCollector
    {
        /// <summary>
        /// Get a list of constants from an expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="constants">list of constants</param>
        /// <returns></returns>
        bool GetConstantExpressions(Expression expression, List<object> constants);
    }

    /// <summary>
    /// class for collecting constants from a Linq Expression
    /// </summary>
    public class ConstantExpressionCollector : IConstantExpressionCollector
    {
        /// <summary>
        /// Get a list of constants from an expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="constants">list of constants</param>
        /// <returns></returns>
        public bool GetConstantExpressions(Expression expression, List<object> constants)
        {
            if (expression == null)
            {
                return true;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return ProcessConstantExpression((ConstantExpression)expression, constants);

                case ExpressionType.New:
                    return ProcessListOfExpression(((NewExpression)expression).Arguments, constants);

                case ExpressionType.MemberInit:
                    return ProcessMemberInit(expression, constants);

                case ExpressionType.MemberAccess:
                    return GetConstantExpressions(((MemberExpression)expression).Expression, constants);

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return GetConstantExpressions(callExpression.Object, constants) &&
                           ProcessListOfExpression(callExpression.Arguments, constants);

                case ExpressionType.NewArrayInit:
                    return ProcessListOfExpression(((NewArrayExpression)expression).Expressions, constants);

                case ExpressionType.Parameter:
                    return true;
            }

            return ProcessDefaultExpressionType(expression, constants);
        }

        private bool ProcessDefaultExpressionType(Expression expression, List<object> constants)
        {
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                return GetConstantExpressions(unaryExpression.Operand, constants);
            }

            var binaryExpression = expression as BinaryExpression;

            if (binaryExpression != null)
            {
                return GetConstantExpressions(binaryExpression.Left, constants) &&
                       GetConstantExpressions(binaryExpression.Right, constants);
            }

            return false;
        }

        private bool ProcessMemberInit(Expression expression, List<object> constants)
        {
            var memberInit = (MemberInitExpression)expression;
            if (!GetConstantExpressions(memberInit.NewExpression, constants))
            {
                return false;
            }

            foreach (var binding in memberInit.Bindings)
            {
                if (binding.BindingType == MemberBindingType.Assignment &&
                    !GetConstantExpressions(((MemberAssignment)binding).Expression, constants))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ProcessConstantExpression(ConstantExpression expression, List<object> constants)
        {
            if (expression.Value != null)
            {
                var valueType = expression.Value.GetType();

                if (valueType == typeof(Delegate))
                {
                    return false;
                }

                if (valueType != typeof(int) &&
                    valueType != typeof(double) &&
                    valueType != typeof(bool) &&
                    valueType != typeof(string) &&
                   !constants.Contains(expression.Value))
                {
                    constants.Add(expression.Value);
                }
            }

            return true;
        }

        private bool ProcessListOfExpression(IEnumerable<Expression> expressions, List<object> constants)
        {
            foreach (var expression in expressions)
            {
                if (!GetConstantExpressions(expression, constants))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
