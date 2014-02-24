using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents an export strategy that can be configured to build a CompiledExportDelegate
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class CompiledExportStrategy : ConfigurableExportStrategy, ICompiledExportStrategy
	{
		protected readonly CompiledExportDelegateInfo delegateInfo;
		protected readonly Attribute[] typeAttributes;
		protected ExportActivationDelegate activationDelegate;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportType"></param>
		public CompiledExportStrategy(Type exportType)
			: base(exportType)
		{
			typeAttributes =
				new List<Attribute>(exportType.GetTypeInfo().GetCustomAttributes(true)).ToArray();

			delegateInfo = new CompiledExportDelegateInfo
			               {
				               ActivationType = exportType,
				               Attributes = typeAttributes
			               };
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			if (Log.IsDebugEnabled)
			{
				if (Lifestyle != null)
				{
					Log.DebugFormat("Activating export type {0} with life cycle container {1} ",
						ActivationType.FullName,
						Lifestyle.GetType().FullName);
				}
				else
				{
					Log.DebugFormat("Activating export type {0} with no life cycle container ",
						ActivationType.FullName);
				}
			}

			if (lifestyle != null)
			{
				return lifestyle.Locate(activationDelegate, exportInjectionScope, context, this);
			}

			return activationDelegate(exportInjectionScope, context);
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

		public void ExportProperty(ExportPropertyInfo exportPropertyInfo)
		{
			PropertyExportStrategy propertyExportStrategy = new PropertyExportStrategy(exportPropertyInfo.PropertyInfo,
				this,
				exportPropertyInfo.ExportCondition);

			if (exportPropertyInfo.ExportNames != null)
			{
				foreach (string exportName in exportPropertyInfo.ExportNames)
				{
					propertyExportStrategy.AddExportName(exportName);
				}
			}

			if (exportPropertyInfo.ExportTypes != null)
			{
				foreach (Type type in exportPropertyInfo.ExportTypes)
				{
					propertyExportStrategy.AddExportType(type);
				}
			}

			AddSecondaryExport(propertyExportStrategy);
		}

		/// <summary>
		/// Configure a method for activation
		/// </summary>
		/// <param name="methodInfo"></param>
		public void ActivateMethod(MethodInfo methodInfo)
		{
			delegateInfo.ActivateMethod(methodInfo);
		}

		/// <summary>
		/// Attributes associated with the export strategy. 
		/// Note: do not return null. Return an empty enumerable if there are none
		/// </summary>
		public override IEnumerable<Attribute> Attributes
		{
			get { return typeAttributes; }
		}

		/// <summary>
		/// When the strategy has been created by a generic strategy this will be set to the strategy that created it
		/// </summary>
		public IExportStrategy CreatingStrategy { get; set; }

		/// <summary>
		/// Specify a particular constructor
		/// </summary>
		/// <param name="constructorInfo"></param>
		public void ImportConstructor(ConstructorInfo constructorInfo)
		{
			delegateInfo.SetImportConstructor(constructorInfo);
		}

		/// <summary>
		/// Adds constructor parameter to the definition
		/// </summary>
		/// <param name="constructorParamInfo"></param>
		public void WithCtorParam(ConstructorParamInfo constructorParamInfo)
		{
			delegateInfo.AddConstructorParamInfo(constructorParamInfo);
		}

		/// <summary>
		/// Adds a cleanup delegate to export strategy
		/// </summary>
		/// <param name="cleanupDelegate"></param>
		public void AddCleanupDelegate(BeforeDisposalCleanupDelegate cleanupDelegate)
		{
			delegateInfo.AddCleanupDelegate(cleanupDelegate);
		}

		/// <summary>
		/// Gets the CompiledExportDelegateInfo definition for this export
		/// </summary>
		/// <returns></returns>
		protected virtual CompiledExportDelegateInfo GetCompiledInfo()
		{
			delegateInfo.IsTransient = lifestyle == null || lifestyle.Transient;

			if (enrichWithDelegates != null)
			{
				foreach (EnrichWithDelegate enrichWithDelegate in enrichWithDelegates)
				{
					delegateInfo.EnrichWithDelegate(enrichWithDelegate);
				}
			}

			if (!ExternallyOwned &&
			    delegateInfo.IsTransient &&
			    typeof(IDisposable).GetTypeInfo().IsAssignableFrom(delegateInfo.ActivationType.GetTypeInfo()))
			{
				delegateInfo.TrackDisposable = true;
			}

			return delegateInfo;
		}
	}
}