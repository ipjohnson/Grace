using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	public interface IValidationRuleResultPart
	{
		IValidationRuleResult RuleResult { get; }

		string PartName { get; }

		ValidationState State { get; }

		string Message { get; }
	}
}
