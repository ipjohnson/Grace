using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
	/// <summary>
	/// Default implementation for ICustomEnrichmentLinqExpressionContext
	/// </summary>
	public class CustomEnrichmentLinqExpressionContext : ICustomEnrichmentLinqExpressionContext
	{
		private List<ParameterExpression> localVariables;

		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="activationType">type being activated</param>
		/// <param name="exportStrategyScopeParameter">injection scope parameter</param>
		/// <param name="injectionContextParameter">injection context parameter</param>
		/// <param name="instanceVariable">instance being constructed</param>
		/// <param name="localVariables">local variables</param>
		public CustomEnrichmentLinqExpressionContext(Type activationType,
																	ParameterExpression exportStrategyScopeParameter,
																	ParameterExpression injectionContextParameter,
																	ParameterExpression instanceVariable,
																	List<ParameterExpression> localVariables)
		{
			ActivationType = activationType;
			ExportStrategyScopeParameter = exportStrategyScopeParameter;
			InjectionContextParameter = injectionContextParameter;
			InstanceVariable = instanceVariable;

			this.localVariables = localVariables;
		}

		/// <summary>
		/// Type being constructed
		/// </summary>
		public Type ActivationType { get; private set; }

		/// <summary>
		/// The IInjectionScope this strategy is being call with
		/// </summary>
		public ParameterExpression ExportStrategyScopeParameter { get; private set; }

		/// <summary>
		/// Injection context parameter (IInjectionContext)
		/// </summary>
		public ParameterExpression InjectionContextParameter { get; private set; }

		/// <summary>
		/// Instance being created (object)
		/// </summary>
		public ParameterExpression InstanceVariable { get; private set; }

		/// <summary>
		/// Parameters added usingt his method will be added to the main body of the delegate
		/// </summary>
		/// <param name="newLocalVariable">new local variable</param>
		public void AddLocalVariable(ParameterExpression newLocalVariable)
		{
			localVariables.Add(newLocalVariable);
		}
	}
}
