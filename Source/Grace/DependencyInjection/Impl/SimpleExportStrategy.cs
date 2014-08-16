using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Export strategy that registers very quickly but doesn't support advanced features like 
	/// importing properties, method or specific values for constructor parameters
	/// </summary>
	public class SimpleExportStrategy : ConfigurableExportStrategy
	{
		private bool trackDisposable;
		protected readonly CompiledExportDelegateInfo delegateInfo;
		private ExportActivationDelegate activationDelegate;
		private IEnumerable<ExportStrategyDependency> dependsOn;

		private static readonly SafeDictionary<Type, Tuple<ExportActivationDelegate, List<ExportStrategyDependency>>>
			delegateDictionary =
				new SafeDictionary<Type, Tuple<ExportActivationDelegate, List<ExportStrategyDependency>>>();

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType">export type</param>
		public SimpleExportStrategy(Type exportType)
			: base(exportType)
		{
			delegateInfo = new CompiledExportDelegateInfo
			               {
				               ActivationType = exportType,
                               Attributes = ImmutableArray<Attribute>.Empty
			               };
		}

		/// <summary>
		/// Initialize strategy
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Tuple<ExportActivationDelegate, List<ExportStrategyDependency>> activationInfo;

			if (!delegateDictionary.TryGetValue(_exportType, out activationInfo))
			{
				SimpleCompiledExportDelegate compiledExportDelegate = new SimpleCompiledExportDelegate(delegateInfo, this);

				activationDelegate = compiledExportDelegate.CompileDelegate();

				dependsOn = compiledExportDelegate.Dependencies;

				delegateDictionary[_exportType] =
					new Tuple<ExportActivationDelegate, List<ExportStrategyDependency>>(activationDelegate,
						compiledExportDelegate.Dependencies);
			}
			else
			{
				activationDelegate = activationInfo.Item1;
				dependsOn = activationInfo.Item2;
			}
		}

		/// <summary>
		/// Depends on strategies
		/// </summary>
		public override IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { return dependsOn ?? base.DependsOn; }
		}

		/// <summary>
		/// activate the strategy
		/// </summary>
		/// <param name="exportInjectionScope">injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">consider filter</param>
		/// <param name="locateKey"></param>
		/// <returns>activated object</returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			if (_lifestyle != null)
			{
				return _lifestyle.Locate(InternalActivate, exportInjectionScope, context, this);
			}

			return InternalActivate(exportInjectionScope, context);
		}

		/// <summary>
		/// Adds constructor parameter to the definition
		/// </summary>
		/// <param name="constructorParamInfo"></param>
		public void WithCtorParam(ConstructorParamInfo constructorParamInfo)
		{
			delegateInfo.AddConstructorParamInfo(constructorParamInfo);
		}

		private object InternalActivate(IInjectionScope injectionscope, IInjectionContext context)
		{
			object returnValue = activationDelegate(injectionscope, context);
			IDisposable disposable = returnValue as IDisposable;

			if (disposable != null && !ExternallyOwned)
			{
				if (context.DisposalScope != null)
				{
					context.DisposalScope.AddDisposable(disposable);
				}
				else
				{
					Log.Error("Activate called with a null DisposalScope in the context");
				}
			}

			if (_enrichWithDelegates != null)
			{
				foreach (EnrichWithDelegate enrichWithDelegate in _enrichWithDelegates)
				{
					returnValue = enrichWithDelegate(injectionscope, context, returnValue);
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Gets the CompiledExportDelegateInfo definition for this export
		/// </summary>
		/// <returns></returns>
		protected virtual CompiledExportDelegateInfo GetCompiledInfo()
		{
			delegateInfo.IsTransient = _lifestyle == null || _lifestyle.Transient;

			if (!ExternallyOwned &&
			    delegateInfo.IsTransient &&
			    typeof(IDisposable).GetTypeInfo().IsAssignableFrom(delegateInfo.ActivationType.GetTypeInfo()))
			{
				trackDisposable = true;
			}

			return delegateInfo;
		}
	}
}