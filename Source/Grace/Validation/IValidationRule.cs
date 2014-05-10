using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// Represents a validation rule
	/// </summary>
	public interface IValidationRule
	{
		/// <summary>
		/// Namespace the rule exists in
		/// </summary>
		string Namespace { get; }

		/// <summary>
		/// Display name for the rule
		/// </summary>
		string DisplayName { get; }

		/// <summary>
		/// Name for the rule
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Properties this rule depends on
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> DependentProperties();

		/// <summary>
		/// Standards this rule is a part of
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> Standards();
	}
}
