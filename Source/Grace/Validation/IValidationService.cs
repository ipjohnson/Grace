using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	public interface IValidationService
	{
		IValidationContext GetValidationContext(object validationObject);

		IValidationContext GetValidationContext(object validationObject, bool monitor);

		IValidationContext GetValidationContext(object validationObject, bool monitor, bool shared);

		IEnumerable<IValidationRule> FetchValidationRules(Type validationType, params string[] standards);

		IEnumerable<string> FetchPropertiesToValidate(Type validationType, params string[] standards);
	}
}
