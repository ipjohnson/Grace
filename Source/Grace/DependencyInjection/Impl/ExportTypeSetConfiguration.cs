using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Grace.LanguageExtensions;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Exports a set of types based on a provided configuration
	/// </summary>
	public class ExportTypeSetConfiguration : IExportTypeSetConfiguration, IExportStrategyProvider
	{
		private readonly List<IExportCondition> conditions;
		private readonly List<Func<Type, bool>> excludeClauses;
		private readonly List<Type> exportBaseTypes;
		private readonly List<Type> exportInterfaces;
		private readonly IInjectionScope injectionScope;
		private readonly List<Func<Type, bool>> interfaceMatchList;
		private readonly IEnumerable<Type> scanTypes;
		private readonly List<Func<Type, bool>> whereClauses;
		private readonly List<IExportStrategyInspector> inspectors; 
		private ILifestyle container;
		private bool exportAllByInterface;
		private bool exportAttributedTypes;
		private ExportEnvironment exportEnvironment;
		private bool externallyOwned;
		private int priority;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="injectionScope"></param>
		/// <param name="scanTypes"></param>
		public ExportTypeSetConfiguration(IInjectionScope injectionScope, IEnumerable<Type> scanTypes)
		{
			this.injectionScope = injectionScope;
			this.scanTypes = scanTypes;
			exportInterfaces = new List<Type>();
			exportBaseTypes = new List<Type>();
			conditions = new List<IExportCondition>();
			excludeClauses = new List<Func<Type, bool>>();
			whereClauses = new List<Func<Type, bool>>();
			interfaceMatchList = new List<Func<Type, bool>>();
			inspectors = new List<IExportStrategyInspector>();
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			List<IExportStrategy> returnValues = new List<IExportStrategy>();

			if (exportInterfaces.Count == 0 &&
			    exportBaseTypes.Count == 0 &&
			    interfaceMatchList.Count == 0 &&
			    !exportAllByInterface)
			{
				exportAttributedTypes = true;
			}

			List<Type> filteredTypes = FilterTypes();

			if (exportAttributedTypes)
			{
				AttributeExportStrategyProvider provider = new AttributeExportStrategyProvider(injectionScope,
					filteredTypes);

				returnValues.AddRange(provider.ProvideStrategies());
			}

			if (exportInterfaces.Count > 0 ||
			    exportBaseTypes.Count > 0 ||
			    interfaceMatchList.Count > 0 ||
			    exportAllByInterface)
			{
				returnValues.AddRange(ScanTypesForExports(filteredTypes));
			}

			if (inspectors.Count > 0)
			{
				foreach (IExportStrategy exportStrategy in returnValues)
				{
					var strategy = exportStrategy;

					inspectors.Apply(x => x.Inspect(strategy));
				}
			}

			return returnValues;
		}

		/// <summary>
		/// Export all objects that implements the specified interface
		/// </summary>
		/// <param name="interfaceType">interface type</param>
		/// <returns>returns self</returns>
		public IExportTypeSetConfiguration ByInterface(Type interfaceType)
		{
			exportInterfaces.Add(interfaceType);

			return this;
		}

		/// <summary>
		/// Export all objects that implements the specified interface
		/// </summary>
		/// <returns>returns self</returns>
		public IExportTypeSetConfiguration ByInterface<T>()
		{
			exportInterfaces.Add(typeof(T));

			return this;
		}

		/// <summary>
		/// Export all classes by interface or that match a set of interfaces
		/// </summary>
		/// <param name="filterMethod"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> filterMethod = null)
		{
			if (filterMethod != null)
			{
				interfaceMatchList.Add(filterMethod);
			}
			else
			{
				exportAllByInterface = true;
			}

			return this;
		}

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <param name="baseType">base type to export</param>
		/// <returns></returns>
		public IExportTypeSetConfiguration BasedOn(Type baseType)
		{
			exportBaseTypes.Add(baseType);

			return this;
		}

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration BasedOn<T>()
		{
			exportBaseTypes.Add(typeof(T));

			return this;
		}

		/// <summary>
		/// Export with the spcified priority
		/// </summary>
		/// <param name="priority">priority to export at</param>
		/// <returns></returns>
		public IExportTypeSetConfiguration WithPriority(int priority)
		{
			this.priority = priority;

			return this;
		}

		/// <summary>
		/// Priority will be set using a priority attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IExportTypeSetConfiguration WithPriorityAttribute<T>() where T : Attribute, IExportPriorityAttribute
		{
			inspectors.Add(new PriorityAttributeInspector<T>());

			return this;
		}

		/// <summary>
		/// Export in the specified Environment
		/// </summary>
		/// <param name="environment">environment to export in</param>
		/// <returns></returns>
		public IExportTypeSetConfiguration InEnvironment(ExportEnvironment environment)
		{
			exportEnvironment = environment;

			return this;
		}

		/// <summary>
		/// Mark all exports as externally owned
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration ExternallyOwned()
		{
			externallyOwned = true;

			return this;
		}

		/// <summary>
		/// Exports are to be marked as shared, similar to a singleton only using a weak reference.
		/// It can not be of type IDisposable
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration AndWeakSingleton()
		{
			container = new WeakSingletonLifestyle();

			return this;
		}

		/// <summary>
		/// Export services as Singletons
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration AndSingleton()
		{
			container = new SingletonLifestyle();

			return this;
		}

		/// <summary>
		/// Set a particular life cycle 
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration WithLifestyle(ILifestyle container)
		{
			this.container = container;

			return this;
		}

		/// <summary>
		/// Export all attributed types
		/// </summary>
		/// <returns></returns>
		public IExportTypeSetConfiguration ExportAttributedTypes()
		{
			exportAttributedTypes = true;

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate)
		{
			conditions.Add(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate)
		{
			conditions.Add(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IExportTypeSetConfiguration AndCondition(IExportCondition condition)
		{
			conditions.Add(condition);

			return this;
		}

		/// <summary>
		/// Exclude a type from being used
		/// </summary>
		/// <param name="exclude"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude)
		{
			excludeClauses.Add(exclude);

			return this;
		}

		/// <summary>
		/// Allows you to filter out types based on the provided where clause
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration Select(Func<Type, bool> whereClause)
		{
			whereClauses.Add(whereClause);

			return this;
		}

		/// <summary>
		/// Adds a new inspector to the export configuration
		/// </summary>
		/// <param name="inspector"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration WithInspector(IExportStrategyInspector inspector)
		{
			inspectors.Add(inspector);

			return this;
		}

		private List<Type> FilterTypes()
		{
			List<Type> filteredTypes = new List<Type>();

			foreach (Type exportedType in scanTypes)
			{
				if (ShouldSkipType(exportedType))
				{
					continue;
				}

				if (excludeClauses.Count > 0)
				{
					bool continueFlag = false;

					foreach (Func<Type, bool> excludeClause in excludeClauses)
					{
						if (excludeClause(exportedType))
						{
							continueFlag = true;
						}
					}

					if (continueFlag)
					{
						continue;
					}
				}

				if (whereClauses.Count > 0)
				{
					bool continueFlag = true;

					foreach (Func<Type, bool> whereClause in whereClauses)
					{
						if (whereClause(exportedType))
						{
							continueFlag = false;
							break;
						}
					}

					if (continueFlag)
					{
						continue;
					}
				}

				filteredTypes.Add(exportedType);
			}

			return filteredTypes;
		}

		private static bool ShouldSkipType(Type exportedType)
		{
			return exportedType.GetTypeInfo().IsInterface ||
			       exportedType.GetTypeInfo().IsAbstract ||
					 typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo()) || 
					 typeof(Exception).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo());
		}

		private IEnumerable<IExportStrategy> ScanTypesForExports(IEnumerable<Type> filteredTypes)
		{
			foreach (Type exportedType in filteredTypes)
			{
				if (exportedType.GetTypeInfo().IsValueType)
				{
					continue;
				}

				IExportStrategy exportStrategy = exportAllByInterface
					? ExportTypeByInterfaces(exportedType)
					: ExportTypeIfMatchesInterface(exportedType);

				if (exportStrategy != null)
				{
					yield return exportStrategy;
				}
			}
		}

		private IExportStrategy ExportTypeByInterfaces(Type exportedType)
		{
			bool generic = false;
			List<Type> exportTypes = new List<Type>();
			IExportStrategy returnValue = null;

			if (exportedType.GetTypeInfo().IsGenericTypeDefinition)
			{
				Type[] genericArgs = exportedType.GetTypeInfo().GenericTypeParameters;

				foreach (Type implementedInterface in exportedType.GetTypeInfo().ImplementedInterfaces)
				{
					if (implementedInterface.IsConstructedGenericType &&
					    implementedInterface.GenericTypeArguments.Length == genericArgs.Length)
					{
						exportTypes.Add(implementedInterface.GetGenericTypeDefinition());
					}
				}

				generic = true;
			}
			else
			{
				exportTypes.AddRange(exportedType.GetTypeInfo().ImplementedInterfaces);
			}

			if (exportTypes.Count > 0)
			{
				returnValue = CreateCompiledExportStrategy(exportedType, generic, false, exportTypes);
			}

			return returnValue;
		}

		private IExportStrategy ExportTypeIfMatchesInterface(Type exportedType)
		{
			if (exportedType.GetTypeInfo().IsGenericTypeDefinition)
			{
				return ExportGenericTypeIfMatchesInterface(exportedType);
			}

			return ExportNonGenericTypeIfMatchesInterface(exportedType);
		}

		private IExportStrategy ExportGenericTypeIfMatchesInterface(Type exportedType)
		{
			List<Type> exportTypes = new List<Type>();
			IExportStrategy returnValue = null;

			bool exportAsSelf = MatchesExportBaseType(exportedType);

			exportTypes.AddRange(FindMatchingExportInterfaces(exportedType));

			if (exportAsSelf || exportTypes.Count > 0)
			{
				returnValue = CreateCompiledExportStrategy(exportedType, true, exportAsSelf, exportTypes);
			}

			return returnValue;
		}

		private IExportStrategy ExportNonGenericTypeIfMatchesInterface(Type exportedType)
		{
			List<Type> exportTypes = new List<Type>();
			IExportStrategy returnValue = null;

			bool exportAsSelf = MatchesExportBaseType(exportedType);

			exportTypes.AddRange(FindMatchingExportInterfaces(exportedType));

			if (exportAsSelf || exportTypes.Count > 0)
			{
				returnValue = CreateCompiledExportStrategy(exportedType, false, exportAsSelf, exportTypes);
			}

			return returnValue;
		}

		private IEnumerable<Type> FindMatchingExportInterfaces(Type exportedType)
		{
			foreach (Type implementedInterface in exportedType.GetTypeInfo().ImplementedInterfaces)
			{
				foreach (Type exportInterface in exportInterfaces)
				{
					if (exportInterface.GetTypeInfo().IsGenericTypeDefinition)
					{
						if (implementedInterface.IsConstructedGenericType &&
						    implementedInterface.GetGenericTypeDefinition() == exportInterface)
						{
							yield return implementedInterface;
						}
					}
					else if (exportInterface.GetTypeInfo().IsAssignableFrom(implementedInterface.GetTypeInfo()))
					{
						yield return exportInterface;
					}
				}

				foreach (Func<Type, bool> func in interfaceMatchList)
				{
					if (func(implementedInterface))
					{
						yield return implementedInterface;
					}
				}
			}
		}

		private bool MatchesExportBaseType(Type exportedType)
		{
			bool matchesBaseType = false;

			foreach (Type exportBaseType in exportBaseTypes)
			{
				if (exportBaseType.GetTypeInfo().IsGenericTypeDefinition)
				{
					Type baseType = exportedType;

					while (baseType != null)
					{
						if (baseType.IsConstructedGenericType)
						{
							matchesBaseType = true;
							break;
						}

						if (baseType == exportBaseType)
						{
							matchesBaseType = true;
							break;
						}

						baseType = baseType.GetTypeInfo().BaseType;
					}
				}
				else
				{
					Type baseType = exportedType;

					while (baseType != null)
					{
						if (baseType == exportBaseType)
						{
							matchesBaseType = true;
							break;
						}

						baseType = baseType.GetTypeInfo().BaseType;
					}
				}
			}

			return matchesBaseType;
		}

		private ICompiledExportStrategy CreateCompiledExportStrategy(Type exportedType,
			bool generic,
			bool exportAsSelf,
			IEnumerable<Type> exportTypes)
		{
			ICompiledExportStrategy exportStrategy;

			if (generic)
			{
				exportStrategy = new GenericExportStrategy(exportedType);
			}
			else
			{
				exportStrategy = new AttributeExportStrategy(exportedType, exportedType.GetTypeInfo().GetCustomAttributes(true));
			}

			if (container != null)
			{
				exportStrategy.SetLifestyleContainer(container.Clone());
			}

			if (externallyOwned)
			{
				exportStrategy.SetExternallyOwned();
			}

			exportStrategy.SetPriority(priority);
			exportStrategy.SetEnvironment(exportEnvironment);

			if (exportAsSelf)
			{
				exportStrategy.AddExportType(exportedType);
			}

			foreach (Type exportType in exportTypes)
			{
				exportStrategy.AddExportType(exportType);
			}

			return exportStrategy;
		}
	}
}