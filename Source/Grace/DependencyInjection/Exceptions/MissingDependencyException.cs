using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// Exception thrown when there is a missing dependency for an export
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

		}

		/// <summary>
		/// Exception message
		/// </summary>
		public override string Message
		{
			get { return string.Format("Could not locate {0} for {1} on {2}", LocateDisplayString,InjectionContext.TargetInfo.InjectionTargetName,InjectionContext.TargetInfo.InjectionType.FullName); }
		}
	}
}
