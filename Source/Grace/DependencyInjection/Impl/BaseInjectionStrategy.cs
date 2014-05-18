using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public class BaseInjectionStrategy : IInjectionStrategy
	{
		private const string CONTEXT_KEY = "ObjectToInject";
		private ExportActivationDelegate activationDelegate;
		private readonly CompiledExportDelegateInfo delegateInfo;

		public BaseInjectionStrategy(Type targeType)
		{
			delegateInfo = new CompiledExportDelegateInfo
			               {
				               ActivationType = targeType,
				               Attributes = targeType.GetTypeInfo().GetCustomAttributes()
			               };

			TargeType = targeType;
		}

		public virtual void Initialize()
		{
			FuncCompiledExportDelegate exportDelegate = new FuncCompiledExportDelegate(delegateInfo,
				(scope, context) => context.Locate(CONTEXT_KEY), null);

			activationDelegate = exportDelegate.CompileDelegate();
		}

		/// <summary>
		/// Target type
		/// </summary>
		public Type TargeType { get; protected set; }

		/// <summary>
		/// Attributes
		/// </summary>
		public IEnumerable<Attribute> Attributes
		{
			get { return delegateInfo.Attributes; }
		}

		public void Inject(IInjectionContext injectionContext, object injectTarget)
		{
			injectionContext.Export(CONTEXT_KEY, (x, y) => injectTarget);

			activationDelegate(injectionContext.RequestingScope, injectionContext);
		}

		/// <summary>
		/// Configure the export to import a method
		/// </summary>
		/// <param name="methodInfo"></param>
		public void ImportMethod(ImportMethodInfo methodInfo)
		{
			delegateInfo.ImportMethod(methodInfo);
		}

		/// <summary>
		/// Configure the export to import a property
		/// </summary>
		/// <param name="propertyInfo"></param>
		public void ImportProperty(ImportPropertyInfo propertyInfo)
		{
			delegateInfo.ImportProperty(propertyInfo);
		}

		/// <summary>
		/// Add enrichment provider
		/// </summary>
		/// <param name="provider"></param>
		public void EnrichmentExpressionProvider(ICustomEnrichmentLinqExpressionProvider provider)
		{
			delegateInfo.EnrichmentExpressionProvider(provider);
		}

		/// <summary>
		/// Mark export to be constructed in a new context
		/// </summary>
		public void InNewContext()
		{
			delegateInfo.InNewContext = true;
		}
	}
}