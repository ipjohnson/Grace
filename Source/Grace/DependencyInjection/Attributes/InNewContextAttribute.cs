using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Classes marked with this attribute will be constructed in a new context
	/// </summary>
	public class InNewContextAttribute : Attribute, IInNewContextAttribute
	{
	}
}
