using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Utilities;

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


		/// <summary>
		/// Exception message
		/// </summary>
		public override string Message
		{
			get
			{
				StringBuilder outputString = new StringBuilder();

				string dependencyType = null;

				if (InjectionContext.TargetInfo.InjectionTarget is ParameterInfo)
				{
					dependencyType = "parameter";
				}
				else
				{
					dependencyType = "property";
				}
					
				outputString.AppendFormat("Could not locate {0} for {1} {2} on {3}{4}{4}",
					LocateDisplayString,
					dependencyType,
					InjectionContext.TargetInfo.InjectionTargetName,
					InjectionContext.TargetInfo.InjectionType.Name,
					Environment.NewLine);

				CreateMessageFromLocationInformation(outputString);

				return outputString.ToString();
			}
		}
	}
}
