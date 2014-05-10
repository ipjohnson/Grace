using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// classes thar implement this interface can provide validation rules for a type
	/// </summary>
	public interface IValidationRuleProvider
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectType"></param>
		/// <param name="standards"></param>
		/// <returns></returns>
		IEnumerable<IValidationRule> ProvideRules(Type objectType, IEnumerable<string> standards);

		IEnumerable<string> GetPropertiesToValidate(Type objectType, IEnumerable<string> standards);
	}
}
