using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This class exports a type using linq expressions
	/// </summary>
	public class CompiledInstanceExportStrategy : CompiledExportStrategy
	{
		private IEnumerable<ExportStrategyDependency> dependencies;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType"></param>
		public CompiledInstanceExportStrategy(Type exportType) : base(exportType)
		{
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			CompiledExportDelegateInfo info = GetCompiledInfo();

			InstanceCompiledExportDelegate delegateGenerator =
				new InstanceCompiledExportDelegate(info,this, OwningScope);

			activationDelegate = delegateGenerator.CompileDelegate();

			dependencies = delegateGenerator.Dependencies;
		}

		/// <summary>
		/// List of dependencies for this strategy
		/// </summary>
		public override IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { return dependencies; }
		}
	}
}