using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
	public class SimpleCompiledExportDelegate : InstanceCompiledExportDelegate
	{
		public SimpleCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo) :
			base(exportDelegateInfo,null)
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

			bodyExpressions.Add(Expression.Call(injectionContextParameter, decrementResolveDepth));

			// only add the return expression if there was no enrichment
			CreateReturnExpression();

			List<Expression> methodExpressions = new List<Expression>();

			methodExpressions.Add(Expression.Call(injectionContextParameter, incrementResolveDepth));

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
