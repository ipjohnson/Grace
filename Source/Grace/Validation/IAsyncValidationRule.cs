using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// asynchronous validation rule
	/// </summary>
	public interface IAsyncValidationRule : IValidationRule
	{
		/// <summary>
		/// Validate an object asynchronous
		/// </summary>
		/// <param name="ruleExecutionContext">execution context</param>
		/// <returns>task</returns>
		Task Validate(IRuleExecutionContext ruleExecutionContext);
	}
}
