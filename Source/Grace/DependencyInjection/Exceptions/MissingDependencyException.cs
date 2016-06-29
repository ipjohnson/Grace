using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Data;
using Grace.Utilities;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// Exception thrown when a null is found when a value is required.
	/// </summary>
	public class MissingDependencyException : LocateException
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="locateName"></param>
		/// <param name="locatingType"></param>
		/// <param name="currentContext"></param>
		public MissingDependencyException(string locateName, Type locatingType, IInjectionContext currentContext) :
			base(locateName, locatingType, currentContext)
		{
			AddLocationInformationEntry(new LocationInformationEntry(locateName,locatingType,currentContext.TargetInfo));
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="locateName"></param>
		/// <param name="locatingType"></param>
		/// <param name="currentContext"></param>
		public MissingDependencyException(string locateName, TypeWrapper locatingType, IInjectionContext currentContext) :
			base(locateName, locatingType, currentContext)
		{
			AddLocationInformationEntry(new LocationInformationEntry(locateName, locatingType, currentContext.TargetInfo));
		}        
	}
}
