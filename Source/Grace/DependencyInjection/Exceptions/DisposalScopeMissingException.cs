using System;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// This exception is thrown when an IDisposable object is requested and no disposal scope is present.
	/// </summary>
	public class DisposalScopeMissingException : Exception
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="activationType"></param>
		public DisposalScopeMissingException(Type activationType) :
			base(string.Format("Activate type {0} without disposal scope", activationType))
		{
		}
	}
}