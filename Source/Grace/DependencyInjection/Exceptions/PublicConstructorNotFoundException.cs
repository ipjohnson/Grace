using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// Exception is thrown when an exported type does not have a public constructor
	/// </summary>
	public class PublicConstructorNotFoundException : Exception
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType"></param>
		public PublicConstructorNotFoundException(Type exportType)
			: base(string.Format("Exported Type {0} does not have a public constructor to use", exportType.FullName))
		{

		}
	}
}
