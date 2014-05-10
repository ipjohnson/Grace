using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation.Attributes
{
	/// <summary>
	/// Base validation attribute
	/// </summary>
	public abstract class SynchronousBaseValidationAttribute : Attribute, ISyncValidationRule
	{
		public string Namespace { get; set; }

		public string DisplayName { get; set; }

		public string Name { get; private set; }

		public virtual IEnumerable<string> DependentProperties()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<string> Standards()
		{
			throw new NotImplementedException();
		}

		public void Validate(IRuleExecutionContext ruleExecutionContext)
		{
			throw new NotImplementedException();
		}
	}
}
