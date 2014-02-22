using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This is the base export class, it provides method to configure itself
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplayString,nq}", Name = "Export")]
	[DebuggerTypeProxy(typeof(ConfigurableExportStrategyDiagnostic))]
	public abstract class ConfigurableExportStrategy : IConfigurableExportStrategy
	{
		private ILog log;
		private bool allowingFiltering = true;
		protected List<IExportCondition> conditions;
		protected bool disposed;
		protected List<EnrichWithDelegate> enrichWithDelegates;
		protected List<string> exportNames;
		protected List<Type> exportTypes;
		protected Type exportType;
		protected ILifestyle lifestyle;
		private bool locked;
		protected ExportMetadata metadata;
		protected List<IExportStrategy> secondaryExports;

		protected ConfigurableExportStrategy(Type exportType)
		{
			ActivationName = exportType.FullName;
			ActivationType = exportType;
			exportNames = new List<string>();
			exportTypes = new List<Type>();
			this.exportType = exportType;

			Environment = ExportEnvironment.Any;

			//log = Logger.GetLogger(GetType());
		}

		/// <summary>
		/// Dispose this strategy
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Locks the export for any more changes
		/// </summary>
		public virtual void Lock()
		{
			locked = true;
		}

		/// <summary>
		/// Activation Type for this export
		/// </summary>
		public Type ActivationType { get; private set; }

		/// <summary>
		/// Usually the type.FullName, used for blacklisting purposes
		/// </summary>
		public string ActivationName { get; private set; }

		/// <summary>
		/// When considering an export should it be filtered out.
		/// True by default, usually it's only false for special export types like Array ad List
		/// </summary>
		public virtual bool AllowingFiltering
		{
			get { return allowingFiltering; }
			protected set { allowingFiltering = value; }
		}

		/// <summary>
		/// Attributes associated with the export strategy. 
		/// Note: do not return null. Return an empty enumerable if there are none
		/// </summary>
		public virtual IEnumerable<Attribute> Attributes
		{
			get { return new Attribute[0]; }
		}

		/// <summary>
		/// The scope that owns this export
		/// </summary>
		public IInjectionScope OwningScope { get; set; }

		/// <summary>
		/// Export Key
		/// </summary>
		public virtual object Key { get; private set; }

		/// <summary>
		/// Set the key value for the strategy
		/// </summary>
		/// <param name="key"></param>
		public void SetKey(object key)
		{
			Key = key;
		}

		/// <summary>
		/// Names this strategy should be known as.
		/// </summary>
		public virtual IEnumerable<string> ExportNames
		{
			get { return exportNames; }
		}

		/// <summary>
		/// Type this strategy should be known as
		/// </summary>
		public IEnumerable<Type> ExportTypes
		{
			get
			{
				if (exportTypes.Count == 0 && exportNames.Count == 0)
				{
					return new[] { exportType };
				}

				return exportTypes;
			}
		}

		/// <summary>
		/// Add an export name for strategy
		/// </summary>
		/// <param name="exportName"></param>
		public virtual void AddExportName(string exportName)
		{
			if (locked)
			{
				throw new ArgumentException("Strategy is locked can't be changed");
			}

			exportNames.Add(exportName);
		}

		/// <summary>
		/// Add an export type for the strategy
		/// </summary>
		/// <param name="exportType"></param>
		public virtual void AddExportType(Type exportType)
		{
			if (locked)
			{
				throw new ArgumentException("Strategy is locked can't be changed");
			}

			exportTypes.Add(exportType);
		}

		public virtual ExportEnvironment Environment { get; private set; }

		/// <summary>
		/// Set the export environment for the strategy
		/// </summary>
		/// <param name="environment"></param>
		public virtual void SetEnvironment(ExportEnvironment environment)
		{
			Environment = environment;
		}

		/// <summary>
		/// What export priority is this being exported as
		/// </summary>
		public virtual int Priority { get; private set; }

		/// <summary>
		/// Set the priority for the strategy
		/// </summary>
		/// <param name="priority"></param>
		public virtual void SetPriority(int priority)
		{
			Priority = priority;
		}

		public void SetExternallyOwned()
		{
			ExternallyOwned = true;
		}

		/// <summary>
		/// ILifestyle associated with export
		/// </summary>
		public virtual ILifestyle Lifestyle
		{
			get { return lifestyle; }
		}

		/// <summary>
		/// Set the life cycle container for the strategy
		/// </summary>
		/// <param name="container"></param>
		public virtual void SetLifestyleContainer(ILifestyle container)
		{
			if (lifestyle == null)
			{
				lifestyle = container;
			}
			else
			{
				throw new Exception("Lifestyle container already configured for this export");
			}
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public virtual void Initialize()
		{
			Lock();
		}

		/// <summary>
		/// Add a condition to the export
		/// </summary>
		/// <param name="exportCondition"></param>
		public virtual void AddCondition(IExportCondition exportCondition)
		{
			if (locked)
			{
				throw new ArgumentException("Strategy is locked, can't be changed");
			}

			if (conditions == null)
			{
				conditions = new List<IExportCondition>(1);
			}

			conditions.Add(exportCondition);
		}

		/// <summary>
		/// Does this export meet the conditions to be used
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		public virtual bool MeetsCondition(IInjectionContext injectionContext)
		{
			if (conditions != null)
			{
				List<IExportCondition> currentConditions = conditions;

				for (int i = 0; i < currentConditions.Count; i++)
				{
					if (!currentConditions[i].ConditionMeet(OwningScope, injectionContext, this))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// An export can specify it's own strategy
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			if (secondaryExports != null)
			{
				return secondaryExports;
			}

			return new IExportStrategy[0];
		}

		/// <summary>
		/// Adds an enrich with delegate to the pipeline
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		public virtual void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{
			if (enrichWithDelegates == null)
			{
				enrichWithDelegates = new List<EnrichWithDelegate>();
			}

			enrichWithDelegates.Add(enrichWithDelegate);
		}

		/// <summary>
		/// List of dependencies for this strategy
		/// </summary>
		public virtual IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { return new ExportStrategyDependency[0]; }
		}

		/// <summary>
		/// Metadata for export
		/// </summary>
		public IExportMetadata Metadata
		{
			get
			{
				if (metadata != null)
				{
					return metadata;
				}

				return new ExportMetadata(Key, new Dictionary<string, object>());
			}
		}

		/// <summary>
		/// Does this export have any conditions, this is important when setting up the strategy
		/// </summary>
		public bool HasConditions
		{
			get { return conditions != null && conditions.Count > 0; }
		}

		/// <summary>
		/// Are the object produced by this export externally owned
		/// </summary>
		public bool ExternallyOwned { get; private set; }

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public abstract object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider);

		/// <summary>
		/// Add metadata to the export
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddMetadata(string name, object value)
		{
			if (metadata == null)
			{
				metadata = new ExportMetadata(Key, new Dictionary<string, object> { { name, value } });
			}
			else
			{
				IDictionary<string, object> newDict = new Dictionary<string, object>();

				foreach (KeyValuePair<string, object> keyValuePair in metadata)
				{
					newDict.Add(keyValuePair);
				}

				newDict[name] = value;

				metadata = new ExportMetadata(Key, newDict);
			}
		}

		/// <summary>
		/// Dispose of this object
		/// </summary>
		/// <param name="dispose"></param>
		protected virtual void Dispose(bool dispose)
		{
			if (disposed)
			{
				return;
			}

			if (dispose)
			{
				ILifestyle lifestyleTemp = lifestyle;

				lifestyle = null;

				if (lifestyleTemp != null)
				{
					lifestyleTemp.Dispose();
				}

				conditions = null;
				enrichWithDelegates = null;

				disposed = true;
			}
		}

		protected void AddSecondaryExport(IExportStrategy strategy)
		{
			if (secondaryExports == null)
			{
				secondaryExports = new List<IExportStrategy>();
			}

			secondaryExports.Add(strategy);
		}

		private string DebuggerDisplayString
		{
			get
			{
				string returnValue = ActivationType.FullName;

				if (exportNames.Count > 0)
				{
					string exportName = exportNames[0];
					int periodIndex = exportName.LastIndexOf('.');

					if (periodIndex > 0)
					{
						exportName = exportName.Substring(periodIndex + 1);
					}

					returnValue += "  as  " + exportName;

					if (exportNames.Count > 1)
					{
						returnValue += " ...";
					}
				}

				return returnValue;
			}
		}

		/// <summary>
		/// Logger for strategies
		/// </summary>
		protected ILog Log
		{
			get { return log ?? (log = Logger.GetLogger(GetType())); }
		}
	}
}