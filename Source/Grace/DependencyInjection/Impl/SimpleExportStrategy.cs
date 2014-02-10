using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
	public class SimpleExportStrategy : ConfigurableExportStrategy
	{
		private bool trackDisposable;
		protected readonly CompiledExportDelegateInfo delegateInfo;
		private ExportActivationDelegate activationDelegate;
		private IEnumerable<ExportStrategyDependency> dependsOn; 
		private static readonly SafeDictionary<Type,Tuple<ExportActivationDelegate,List<ExportStrategyDependency>>> delegateDictionary =
			new SafeDictionary<Type, Tuple<ExportActivationDelegate, List<ExportStrategyDependency>>>();

		public SimpleExportStrategy(Type exportType)
			: base(exportType)
		{
			delegateInfo = new CompiledExportDelegateInfo
			               {
				               ActivationType = exportType,
									Attributes = new Attribute[0]
			               };
		}

		public override void Initialize()
		{
			base.Initialize();

			Tuple<ExportActivationDelegate, List<ExportStrategyDependency>> activationInfo;

			if (!delegateDictionary.TryGetValue(exportType, out activationInfo))
			{
				SimpleCompiledExportDelegate compiledExportDelegate = new SimpleCompiledExportDelegate(delegateInfo);

				activationDelegate = compiledExportDelegate.CompileDelegate();

				dependsOn = compiledExportDelegate.Dependencies;

				delegateDictionary[exportType] = 
					new Tuple<ExportActivationDelegate, List<ExportStrategyDependency>>(activationDelegate, compiledExportDelegate.Dependencies);
			}
			else
			{
				activationDelegate = activationInfo.Item1;
				dependsOn = activationInfo.Item2;
			}
		}

		public override IEnumerable<ExportStrategyDependency> DependsOn
		{
			get
			{
				return dependsOn ?? base.DependsOn;
			}
		}

		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			if (lifestyle != null)
			{
				return lifestyle.Locate(InternalActivate, exportInjectionScope, context, this);
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

			if (enrichWithDelegates != null)
			{
				foreach (EnrichWithDelegate enrichWithDelegate in enrichWithDelegates)
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
			delegateInfo.IsTransient = lifestyle == null || lifestyle.Transient;

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
