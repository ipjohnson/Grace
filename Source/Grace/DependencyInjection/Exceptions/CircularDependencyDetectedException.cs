using System;
using System.Reflection;
using System.Text;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// This exception is thrown when a loop is detected trying to resolve an export.
	/// </summary>
	public class CircularDependencyDetectedException : LocateException
	{
		public CircularDependencyDetectedException(string locateName, Type locatingType, IInjectionContext currentContext) : base(locateName, locatingType, currentContext)
		{

		}

		public override string Message
		{
			get
			{
				StringBuilder outputString = new StringBuilder();

				outputString.AppendFormat("Circular dependency detected see stack for error below");

				outputString.AppendLine();

				CreateMessageFromLocationInformation(outputString);

				return outputString.ToString();
			}
		}
	}
}