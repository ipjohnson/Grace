using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Information to export a property on an object
	/// </summary>
	public class ExportPropertyInfo
	{
		/// <summary>
		/// Property to export
		/// </summary>
		public PropertyInfo PropertyInfo { get; set; }

		/// <summary>
		/// Export Type
		/// </summary>
		public IEnumerable<Type> ExportTypes { get; set; }

		/// <summary>
		/// Export names
		/// </summary>
		public IEnumerable<string> ExportNames { get; set; }

		/// <summary>
		/// Export Condition
		/// </summary>
		public IExportCondition ExportCondition { get; set; }

		/// <summary>
		/// Add a type to the export info
		/// </summary>
		/// <param name="exporType"></param>
		public void AddExportType(Type exporType)
		{
			if (ExportTypes == null)
			{
				ExportTypes = new List<Type> { exporType };
			}
			else
			{
				ExportTypes = new List<Type>(ExportTypes) { exporType };
			}
		}

		/// <summary>
		/// Add an export name to export property
		/// </summary>
		/// <param name="exportName"></param>
		public void AddExportName(string exportName)
		{
			if (ExportNames == null)
			{
				ExportNames = new List<string> { exportName };
			}
			else
			{
				ExportNames = new List<string>(ExportNames) { exportName };
			}
		}
	}

	/// <summary>
	/// This interface represents an export strategy that can be configured for importing
	/// </summary>
	public interface ICompiledExportStrategy : IConfigurableExportStrategy
	{
		/// <summary>
		/// When the strategy has been created by a generic strategy this will be set to the strategy that created it
		/// </summary>
		IExportStrategy CreatingStrategy { get; set; }

		/// <summary>
		/// Configure the export to import a method
		/// </summary>
		/// <param name="methodInfo"></param>
		void ImportMethod(ImportMethodInfo methodInfo);

		/// <summary>
		/// Configure the export to import a property
		/// </summary>
		void ImportProperty(ImportPropertyInfo importPropertyInfo);

		/// <summary>
		/// mark a property for exporting
		/// </summary>
		/// <param name="exportProperty"></param>
		void ExportProperty(ExportPropertyInfo exportProperty);

		/// <summary>
		/// Configure a method for activation
		/// </summary>
		/// <param name="methodInfo"></param>
		void ActivateMethod(MethodInfo methodInfo);

		/// <summary>
		/// Specify a particular constructor
		/// </summary>
		/// <param name="constructorInfo"></param>
		void ImportConstructor(ConstructorInfo constructorInfo);

		/// <summary>
		/// Adds constructor parameter to the definition
		/// </summary>
		/// <param name="constructorParamInfo"></param>
		void WithCtorParam(ConstructorParamInfo constructorParamInfo);

		/// <summary>
		/// Adds a cleanup delegate to export strategy
		/// </summary>
		/// <param name="cleanupDelegate"></param>
		void AddCleanupDelegate(BeforeDisposalCleanupDelegate cleanupDelegate);
		
		/// <summary>
		/// Adds custom provider
		/// </summary>
		/// <param name="provider"></param>
		void EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider);

		/// <summary>
		/// Export class in new injection context
		/// </summary>
		void InNewContext();
	}
}