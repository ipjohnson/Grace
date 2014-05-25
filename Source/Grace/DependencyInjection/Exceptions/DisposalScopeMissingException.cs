using System;
using System.Text;
using Grace.Utilities;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// This exception is thrown when an IDisposable object is requested and no disposal scope is present.
	/// </summary>
	public class DisposalScopeMissingException : LocateException
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="activationType"></param>
		/// <param name="injectionContext"></param>
		public DisposalScopeMissingException(Type activationType, IInjectionContext injectionContext) :
			base(null,null,injectionContext)
		{
			ActivationType = activationType;
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="activationType"></param>
		/// <param name="injectionContext"></param>
		public DisposalScopeMissingException(TypeWrapper activationType, IInjectionContext injectionContext) :
			base(null, null, injectionContext)
		{
			ActivationType = activationType;
		}

		/// <summary>
		/// Type that was being activated
		/// </summary>
		public Type ActivationType { get; private set; }

		/// <summary>
		/// Message for exception
		/// </summary>
		public override string Message
		{
			get
			{
				StringBuilder builder = new StringBuilder();

				builder.AppendFormat("Disposal Scope was missing when activating type {0}{1}",
					ActivationType.Name,
					Environment.NewLine);

				builder.AppendLine();

				CreateMessageFromLocationInformation(builder);

				return builder.ToString();
			}
		}
	}
}