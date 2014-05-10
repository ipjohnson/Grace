using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// Represents a standard validation rule that runs synchronously 
	/// </summary>
	public interface ISyncValidationRule : IValidationRule
	{
		/// <summary>
		/// Validate an object
		/// </summary>
		/// <param name="ruleExecutionContext">rule execution context</param>
		void Validate(IRuleExecutionContext ruleExecutionContext);
	}
}
