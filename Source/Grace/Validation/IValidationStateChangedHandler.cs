using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	public interface IValidationStateChangedHandler
	{
		void StateChanged(IValidationContext context, ValidationState validationState);
	}
}
