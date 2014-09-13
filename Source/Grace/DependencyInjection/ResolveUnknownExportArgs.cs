using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// This event arg can be handled to resolve unknown types.
	/// </summary>
	public class ResolveUnknownExportArgs : EventArgs
	{
	    /// <summary>
	    /// CSTOR
	    /// </summary>
	    /// <param name="injectionContext"></param>
	    /// <param name="requestedName"></param>
	    /// <param name="requestedType"></param>
	    /// <param name="locateKey"></param>
	    public ResolveUnknownExportArgs(IInjectionContext injectionContext, string requestedName, Type requestedType, object locateKey)
		{
			InjectionContext = injectionContext;
			RequestedName = requestedName;
			RequestedType = requestedType;
		    LocateKey = locateKey;
		}

		/// <summary>
		/// Injection context for the resolve request
		/// </summary>
		public IInjectionContext InjectionContext { get; private set; }

		/// <summary>
		/// The name of the export that was requested. Typically the full type name
		/// </summary>
		public string RequestedName { get; private set; }

		/// <summary>
		/// The type that was requested, can be null if requested by name only
		/// </summary>
		public Type RequestedType { get; private set; }

        /// <summary>
        /// Locate key
        /// </summary>
        public object LocateKey { get; private set; }

		/// <summary>
		/// You can provide an export value
		/// </summary>
		public object ExportedValue { get; set; }
	}
}