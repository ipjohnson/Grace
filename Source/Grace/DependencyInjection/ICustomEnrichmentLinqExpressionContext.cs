using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Provides information about the expression being constructed
	/// </summary>
	public interface ICustomEnrichmentLinqExpressionContext
	{
		/// <summary>
		/// Type being constructed
		/// </summary>
		Type ActivationType { get;  }

		/// <summary>
		/// The IInjectionScope this strategy is being call with
		/// </summary>
		ParameterExpression ExportStrategyScopeParameter { get; }

		/// <summary>
		/// Injection context parameter (IInjectionContext)
		/// </summary>
		ParameterExpression InjectionContextParameter { get;  }

		/// <summary>
		/// Instance being created (object)
		/// </summary>
		ParameterExpression InstanceVariable { get; }

		/// <summary>
		/// Parameters added usingt his method will be added to the main body of the delegate
		/// </summary>
		/// <param name="newLocalVariable">new local variable</param>
		void AddLocalVariable(ParameterExpression newLocalVariable);
	}
}
