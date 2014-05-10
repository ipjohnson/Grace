using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// Represents the validation state for an object or property
	/// </summary>
	public enum ValidationState
	{
		/// <summary>
		/// Valid
		/// </summary>
		Valid = 1,

		/// <summary>
		/// Valid but with a warning
		/// </summary>
		ValidWithWarning = 2,

		/// <summary>
		/// Invalid
		/// </summary>
		Invalid = 3,

		/// <summary>
		/// Invalid because it's required and empty
		/// </summary>
		InvalidRequired = 4
	}
}
