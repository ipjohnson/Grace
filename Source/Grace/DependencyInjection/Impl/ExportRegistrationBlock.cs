using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This class implements the IExportRegistrationBlock interface and provides exports for IInjectionScope
	/// Note: this class is not thread safe. You can call configure from multiple threads on the same scope
	/// but you can not call from multiple threads to the same instance of a registration block
	/// </summary>
	public class ExportRegistrationBlock : IExportRegistrationBlock
	{
		private ILog log;
		private readonly IInjectionScope owningScope;
		private readonly List<IExportStrategyProvider> strategyProviders = new List<IExportStrategyProvider>();
		private ExportStrategyListProvider exportStrategyList;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="owningScope"></param>
		public ExportRegistrationBlock(IInjectionScope owningScope)
		{
			this.owningScope = owningScope;
		}

		/// <summary>
		/// Scope this registration block is for
		/// </summary>
		public IInjectionScope OwningScope
		{
			get { return owningScope; }
		}

		private ILog Log
		{
			get { return log ?? (log = Logger.GetLogger<ExportRegistrationBlock>()); }
		}

		/// <summary>
		/// Register an export by it's type. This is required when dealing with open generics
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration Export(Type type)
		{
			ICompiledExportStrategy compiledExportStrategy;

			if (type.GetTypeInfo().IsGenericTypeDefinition)
			{
				compiledExportStrategy = new GenericExportStrategy(type);
			}
			else
			{
				compiledExportStrategy = new CompiledInstanceExportStrategy(type);
			}

			FluentExportStrategyConfiguration exportStrategy =
				new FluentExportStrategyConfiguration(type, compiledExportStrategy);

			strategyProviders.Add(exportStrategy);

			return exportStrategy;
		}

		/// <summary>
		/// Register an export by it's type. This method allows you to specify things using linq expressions
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> Export<T>()
		{
			CompiledInstanceExportStrategy strategy = new CompiledInstanceExportStrategy(typeof(T));

			FluentExportStrategyConfiguration<T> exportStrategy =
				new FluentExportStrategyConfiguration<T>(strategy);

			strategyProviders.Add(exportStrategy);

			return exportStrategy;
		}

		/// <summary>
		/// Register a collection of types all at one time
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration Export(IEnumerable<Type> types)
		{
			ExportTypeSetConfiguration configuration = new ExportTypeSetConfiguration(owningScope, types);

			strategyProviders.Add(configuration);

			return configuration;
		}

		/// <summary>
		/// Register an assembly for exports. You can perform any of these
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration ExportAssembly(Assembly assembly)
		{
			ExportTypeSetConfiguration configuration = new ExportTypeSetConfiguration(owningScope, assembly.ExportedTypes);

			strategyProviders.Add(configuration);

			return configuration;
		}

		/// <summary>
		/// Register types from an assembly containing a particular type
		/// </summary>
		/// <typeparam name="T">type in assembly you want to export</typeparam>
		/// <returns>set configuration object</returns>
		public IExportTypeSetConfiguration ExportAssemblyContaining<T>()
		{
			ExportTypeSetConfiguration configuration = new ExportTypeSetConfiguration(owningScope, typeof(T).GetTypeInfo().Assembly.ExportedTypes);

			strategyProviders.Add(configuration);

			return configuration;
		}

		/// <summary>
		/// Register a set of assemblies.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public IExportTypeSetConfiguration ExportAssemblies(IEnumerable<Assembly> assemblies)
		{
			List<Type> types = new List<Type>();

			foreach (Assembly assembly in assemblies)
			{
				types.AddRange(assembly.ExportedTypes);
			}

			ExportTypeSetConfiguration configuration = new ExportTypeSetConfiguration(owningScope, types);

			strategyProviders.Add(configuration);

			return configuration;
		}

		/// <summary>
		/// Export an instance of an object for a particular set of interfaces
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		public IFluentExportInstanceConfiguration<T> ExportInstance<T>(T instance)
		{
			InstanceStrategy<T> instanceStrategy = new InstanceStrategy<T>(instance);

			ExportInstanceConfiguration<T> returnValue =
				new ExportInstanceConfiguration<T>(typeof(T), instanceStrategy);

			strategyProviders.Add(returnValue);

			return returnValue;
		}

		/// <summary>
		/// Export an instance of an object for a particular set of interfaces
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceFunction"></param>
		public IFluentExportInstanceConfiguration<T> ExportInstance<T>(ExportFunction<T> instanceFunction)
		{
			InstanceFuncStrategy<T> instanceFuncStrategy = new InstanceFuncStrategy<T>(instanceFunction);

			ExportInstanceConfiguration<T> returnValue =
				new ExportInstanceConfiguration<T>(typeof(T), instanceFuncStrategy);

			strategyProviders.Add(returnValue);

			return returnValue;
		}

		/// <summary>
		/// Register an export function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exportFunction"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ExportFunc<T>(ExportFunction<T> exportFunction)
		{
			CompiledFuncExportStrategy<T> funcExport = new CompiledFuncExportStrategy<T>((x, y) => exportFunction(x, y));

			FluentExportStrategyConfiguration<T> returnValue = new FluentExportStrategyConfiguration<T>(funcExport);

			strategyProviders.Add(returnValue);

			return returnValue;
		}

		public IFluentSimpleExportStrategyConfiguration SimpleExport<T>()
		{
			SimpleExportStrategy exportStrategy = new SimpleExportStrategy(typeof(T));

			FluentSimpleExportStrategyConfiguration configuration = new FluentSimpleExportStrategyConfiguration(exportStrategy);

			strategyProviders.Add(configuration);

			return configuration;
		}

		public IFluentSimpleExportStrategyConfiguration SimpleExport(Type type)
		{
			ConfigurableExportStrategy exportStrategy = null;

			if (type.GetTypeInfo().IsGenericTypeDefinition)
			{
				exportStrategy = new SimpleGenericExportStrategy(type);
			}
			else
			{
				exportStrategy = new SimpleExportStrategy(type);
			}

			FluentSimpleExportStrategyConfiguration configuration = new FluentSimpleExportStrategyConfiguration(exportStrategy);

			strategyProviders.Add(configuration);

			return configuration;
		}

		/// <summary>
		/// Add an export strategy directly to a scope
		/// </summary>
		/// <param name="strategy"></param>
		public void AddExportStrategy(IExportStrategy strategy)
		{
			if (exportStrategyList == null)
			{
				exportStrategyList = new ExportStrategyListProvider();

				strategyProviders.Add(exportStrategyList);
			}

			exportStrategyList.Add(strategy);
		}

		/// <summary>
		/// Using this the developer can provide C# extensions that add to the registration block
		/// </summary>
		/// <param name="strategyProvider">new strategy provider</param>
		public void AddExportProvider(IExportStrategyProvider strategyProvider)
		{
			strategyProviders.Add(strategyProvider);
		}

		/// <summary>
		/// Get all exports for the registration block
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> GetExportStrategies()
		{
			foreach (IExportStrategyProvider exportStrategyProvider in strategyProviders)
			{
				foreach (IExportStrategy provideStrategy in exportStrategyProvider.ProvideStrategies())
				{
					if (FilterOutBasedOnEnvironment(provideStrategy.Environment))
					{
						continue;
					}

					if (Log.IsInfoEnabled)
					{
						string exportNames = null;

						if (provideStrategy.ExportNames.Any())
						{
							provideStrategy.ExportNames.Aggregate((x, y) => string.Concat(x, ',', y));
						}

						Log.InfoFormat("Exporting type {0} as {1}", provideStrategy.ActivationType.FullName, exportNames);
					}

					yield return provideStrategy;
				}
			}
		}

		/// <summary>
		/// Filter out strategies that are exported for particular environments
		/// </summary>
		/// <param name="strategyEnvironment"></param>
		/// <returns></returns>
		private bool FilterOutBasedOnEnvironment(ExportEnvironment strategyEnvironment)
		{
			bool returnValue = false;
			bool runTimeOnly = (strategyEnvironment & ExportEnvironment.RunTimeOnly) == ExportEnvironment.RunTimeOnly;
			bool unitTestOnly = (strategyEnvironment & ExportEnvironment.UnitTestOnly) == ExportEnvironment.UnitTestOnly;
			bool designTimeOnly = (strategyEnvironment & ExportEnvironment.DesignTimeOnly) == ExportEnvironment.DesignTimeOnly;

			switch (owningScope.Environment)
			{
				case ExportEnvironment.RunTime:
					if (!runTimeOnly && (unitTestOnly || designTimeOnly))
					{
						returnValue = true;
					}
					break;

				case ExportEnvironment.UnitTest:
					if (!unitTestOnly && (runTimeOnly || designTimeOnly))
					{
						returnValue = true;
					}
					break;

				case ExportEnvironment.DesignTime:
					if (!designTimeOnly && (unitTestOnly || runTimeOnly))
					{
						returnValue = true;
					}
					break;
			}

			return returnValue;
		}
	}
}