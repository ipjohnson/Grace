using System;
using System.Collections.Generic;
using System.ServiceModel;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.WCF.LanguageExtensions
{
	/// <summary>
	/// Extension class to help store and dispose of data
	/// </summary>
	public class OperationCustomDataExtension : IExtension<OperationContext>, IDisposable
	{
		private DisposalScope disposalScope = new DisposalScope();

		/// <summary>
		/// Default constructor
		/// </summary>
		public OperationCustomDataExtension()
		{
			Items = new Dictionary<object, object>();
		}

		/// <summary>
		/// Shared data
		/// </summary>
		public IDictionary<object, object> Items { get; private set; }

		/// <summary>
		/// Disposal scope
		/// </summary>
		public IDisposalScope DisposalScope
		{
			get { return disposalScope; }
		}

		/// <summary>
		/// Dispose of extension
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
		/// </summary>
		/// <param name="owner">The extensible object that aggregates this extension.</param>
		public void Attach(OperationContext owner)
		{
		}

		/// <summary>
		/// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
		/// </summary>
		/// <param name="owner">The extensible object that aggregates this extension.</param>
		public void Detach(OperationContext owner)
		{
		}

		private void Dispose(bool dispose)
		{
			if (dispose && disposalScope != null)
			{
				disposalScope.Dispose();

				disposalScope = null;

				GC.SuppressFinalize(this);
			}
		}
	}

	/// <summary>
	/// C# extension
	/// </summary>
	public static class OperationContextExtensions
	{
		/// <summary>
		/// Get the current Items for this request
		/// </summary>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public static IDictionary<object, object> Items(this OperationContext operationContext)
		{
			return GetExtension(operationContext).Items;
		}

		/// <summary>
		/// Disposal scope for this request
		/// </summary>
		/// <param name="operationContext"></param>
		/// <returns></returns>
		public static IDisposalScope DisposalScope(this OperationContext operationContext)
		{
			return GetExtension(operationContext).DisposalScope;
		}

		private static OperationCustomDataExtension GetExtension(OperationContext operationContext)
		{
			OperationCustomDataExtension extension = operationContext.Extensions.Find<OperationCustomDataExtension>();

			if (extension == null)
			{
				extension = new OperationCustomDataExtension();

				operationContext.Extensions.Add(extension);
			}

			return extension;
		}
	}
}