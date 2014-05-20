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
			private readonly IInjectionTargetInfo targetInfo;
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
				targetInfo = context.TargetInfo;
			}

			/// <summary>
			/// This the method that is bound to the delegate
			/// </summary>
			/// <returns></returns>
			public TReturn Activate()
			{
				IInjectionContext clonedContext = context.Clone();

				clonedContext.TargetInfo = targetInfo;
				clonedContext.RequestingScope = requestingInjectionScope;
				clonedContext.DisposalScope = disposalScope;

				return requestingInjectionScope.Locate<TReturn>(injectionContext: clonedContext, consider: consider);
			}
		}

		#endregion

		/// <summary>
		/// Activates a new TDelegate by binding to a new helper
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
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