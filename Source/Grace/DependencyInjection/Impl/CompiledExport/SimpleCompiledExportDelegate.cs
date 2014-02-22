using System.Collections.Generic;
using System.Linq.Expressions;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
	public class SimpleCompiledExportDelegate : InstanceCompiledExportDelegate
	{
		public SimpleCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo) :
			base(exportDelegateInfo, null)
		{
		}

		/// <summary>
		/// This method generates the compiled delegate
		/// </summary>
		/// <returns></returns>
		protected override ExportActivationDelegate GenerateDelegate()
		{
			SetUpParameterExpressions();

			SetUpInstanceVariableExpression();

			CreateInstantiationExpression();

			CreateCustomInitializeExpressions();

			bodyExpressions.Add(Expression.Call(injectionContextParameter, DecrementResolveDepth));

			// only add the return expression if there was no enrichment
			CreateReturnExpression();

			List<Expression> methodExpressions = new List<Expression>
			                                     {
				                                     Expression.Call(injectionContextParameter, IncrementResolveDepth)
			                                     };

			methodExpressions.AddRange(GetImportExpressions());
			methodExpressions.AddRange(instanceExpressions);
			methodExpressions.AddRange(bodyExpressions);

			BlockExpression body = Expression.Block(localVariables, methodExpressions);

			return Expression.Lambda<ExportActivationDelegate>(body,
				exportStrategyScopeParameter,
				injectionContextParameter).Compile();
		}
	}
}