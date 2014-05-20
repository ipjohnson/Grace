using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.DelegateFactory
{
	/// <summary>
	/// Export Strategy for delegates that match the signature Func&lt;TArg1,TReturn&gt;
	/// </summary>
	/// <typeparam name="TDelegate"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <typeparam name="TArg1"></typeparam>
	public class GenericDelegateExportStrategy<TDelegate, TReturn, TArg1> : BaseGenericDelegateExportStrategy
	{
		private string argName1;

		/// <summary>
		/// Initialize
		/// </summary>
		public override void Initialize()
		{
			if (InjectionKernel.ImportTypeByName(typeof(TArg1)))
			{
				MethodInfo invoke = typeof(TDelegate).GetTypeInfo().GetDeclaredMethod("Invoke");

				argName1 = invoke.GetParameters().First().Name;
			}
		}

		/// <summary>
		/// Activate the object
		/// </summary>
		/// <param name="exportInjectionScope">injetion scope</param>
		/// <param name="context">context for the activation</param>
		/// <param name="consider">consider filter</param>
		/// <param name="locateKey"></param>
		/// <returns>
		/// activated object
		/// </returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			Helper newHelper = new Helper(context, consider, argName1);

			return activateMethodInfo.CreateDelegate(typeof(TDelegate), newHelper);
		}

		/// <summary>
		/// Activation Type
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
		/// List of export types
		/// </summary>
		public override IEnumerable<Type> ExportTypes
		{
			get { yield return typeof(TDelegate); }
		}

		#region helper

		private static readonly MethodInfo activateMethodInfo;

		static GenericDelegateExportStrategy()
		{
			activateMethodInfo = typeof(Helper).GetRuntimeMethod("Activate", new[] { typeof(TArg1) });
		}

		public class Helper
		{
			private readonly IInjectionContext injectionContext;
			private readonly IInjectionTargetInfo targetInfo;
			private readonly string argName1;
			private readonly ExportStrategyFilter consider;
			private readonly IInjectionScope requestInjectionScope;
			private readonly IDisposalScope disposalScope;

			public Helper(IInjectionContext injectionContext, ExportStrategyFilter consider, string argName1)
			{
				this.injectionContext = injectionContext;
				this.argName1 = argName1;
				this.consider = consider;

				requestInjectionScope = injectionContext.RequestingScope;
				disposalScope = injectionContext.DisposalScope;
				targetInfo = injectionContext.TargetInfo;
			}

			public TReturn Activate(TArg1 arg1)
			{
				IInjectionContext newInjectionContext = injectionContext.Clone();

				newInjectionContext.RequestingScope = requestInjectionScope;
				newInjectionContext.DisposalScope = disposalScope;
				newInjectionContext.TargetInfo = targetInfo;

				if (argName1 != null)
				{
					newInjectionContext.Export(argName1, (s, c) => arg1);
				}
				else
				{
					newInjectionContext.Export((s, c) => arg1);
				}

				return requestInjectionScope.Locate<TReturn>(injectionContext: newInjectionContext, consider: consider);
			}
		}

		#endregion
	}
}