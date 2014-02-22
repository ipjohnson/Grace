using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.DelegateFactory
{
	/// <summary>
	/// Export strategy for any delegate that matches Func&lt;T&gt;
	/// </summary>
	/// <typeparam name="TDelegate">delegate type</typeparam>
	/// <typeparam name="TReturn">delegate return type</typeparam>
	public class GenericDelegateExportStrategy<TDelegate, TReturn> : BaseGenericDelegateExportStrategy
	{
		#region Helper

		private static readonly MethodInfo helperMethodInfo;

		static GenericDelegateExportStrategy()
		{
			helperMethodInfo = typeof(Helper).GetRuntimeMethod("Activate", new Type[0]);
		}

		/// <summary>
		/// Helper class that is used to bind the delegate type to
		/// </summary>
		public class Helper
		{
			private readonly IInjectionContext context;
			private readonly ExportStrategyFilter consider;
			private readonly IInjectionScope requestingInjectionScope;
			private readonly IDisposalScope disposalScope;

			/// <summary>
			/// DEfault constructor
			/// </summary>
			/// <param name="context">injection context</param>
			/// <param name="consider">consider delegate</param>
			public Helper(IInjectionContext context, ExportStrategyFilter consider)
			{
				this.context = context;
				this.consider = consider;

				requestingInjectionScope = context.RequestingScope;
				disposalScope = context.DisposalScope;
			}

			/// <summary>
			/// This the method that is bound to the delegate
			/// </summary>
			/// <returns></returns>
			public TReturn Activate()
			{
				IInjectionContext clonedContext = context.Clone();

				clonedContext.RequestingScope = requestingInjectionScope;
				clonedContext.DisposalScope = disposalScope;

				return requestingInjectionScope.Locate<TReturn>(clonedContext, consider);
			}
		}

		#endregion

		/// <summary>
		/// Activates a new TDelegate by binding to a new helper
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			Helper helper = new Helper(context, consider);

			return helperMethodInfo.CreateDelegate(typeof(TDelegate), helper);
		}

		/// <summary>
		/// Activation type
		/// </summary>
		public override Type ActivationType
		{
			get { return typeof(TDelegate); }
		}

		/// <summary>
		/// Activation name
		/// </summary>
		public override string ActivationName
		{
			get { return typeof(TDelegate).FullName; }
		}

		/// <summary>
		/// Exported types
		/// </summary>
		public override IEnumerable<Type> ExportTypes
		{
			get { yield return typeof(TDelegate); }
		}
	}
}