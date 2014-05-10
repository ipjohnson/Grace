using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	public interface IValidationStandardsSelector
	{
		Type ValidationType();

		IEnumerable<string> Select(object validationObject);

		IEnumerable<string> MonitorProperties();

		event EventHandler<string> StandardsChanged;
	}
}
