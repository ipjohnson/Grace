using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	public static class IValidationRuleResultExtensions
	{
		public static string FullRuleName(this IValidationRuleResult ruleResult)
		{
			return ruleResult.Namespace + "." + ruleResult.RuleName;
		}
	}

	/// <summary>
	/// Validation result for rule
	/// </summary>
	public interface IValidationRuleResult
	{
		/// <summary>
		/// Namspace the rule exists in
		/// </summary>
		string Namespace { get; }

		/// <summary>
		/// Rule name
		/// </summary>
		string RuleName { get; }

		/// <summary>
		/// Object being validated
		/// </summary>
		object ValidationObject { get; }

		/// <summary>
		/// Rule display name
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Validation state
		/// </summary>
		ValidationState State { get; }

		/// <summary>
		/// Validation message
		/// </summary>
		string Message { get; }

		/// <summary>
		/// Root property path
		/// </summary>
		string RootPropertyPath { get; set; }

		/// <summary>
		/// Validation rule parts
		/// </summary>
		/// <returns></returns>
		IEnumerable<IValidationRuleResultPart> ResultParts();
	}
}
