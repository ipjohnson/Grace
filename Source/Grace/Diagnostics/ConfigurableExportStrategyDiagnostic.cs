using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.Diagnostics
{
	public class ConfigurableExportStrategyDiagnostic
	{
		private ConfigurableExportStrategy strategy;

		public ConfigurableExportStrategyDiagnostic(ConfigurableExportStrategy strategy)
		{
			this.strategy = strategy;
		}

		[DebuggerDisplay("{ActivationType}", Name = "Export Type")]
		public Type ActivationType
		{
			get { return strategy.ActivationType; }
		}

		[DebuggerDisplay("{ExportNamesDisplayString,nq}", Name = "Export Names")]
		public IEnumerable<string> ExportNames
		{
			get { return strategy.ExportNames; }
		}

		[DebuggerDisplay("{ExportTypesDisplayString,nq}", Name = "Export Types")]
		public IEnumerable<Type> ExportTypes
		{
			get { return strategy.ExportTypes; }
		}

		public object Key
		{
			get { return strategy.Key; }
		}

		[DebuggerDisplay("{Lifestyle,nq}")]
		public string Lifestyle
		{
			get
			{
				if (strategy.Lifestyle != null)
				{
					return strategy.Lifestyle.GetType().Name;
				}

				return "Transient";
			}
		}

		[DebuggerDisplay("{DependencyDebugDisplayString,nq}", Name = "Depends On")]
		public IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { return strategy.DependsOn; }
		}

		public ExportEnvironment Environment
		{
			get { return strategy.Environment; }
		}

		public int Priority
		{
			get { return strategy.Priority; }
		}

		[DebuggerDisplay("{HasConditionsDisplayString,nq}", Name = "Has Conditions")]
		public bool HasConditons
		{
			get { return strategy.HasConditions; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DependencyDebugDisplayString
		{
			get { return "Count = " + strategy.DependsOn.Count(); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string ExportNamesDisplayString
		{
			get { return "Count = " + strategy.ExportNames.Count(); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string ExportTypesDisplayString
		{
			get { return "Count = " + strategy.ExportTypes.Count(); }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string HasConditionsDisplayString
		{
			get { return strategy.HasConditions.ToString(); }
		}
	}
}
