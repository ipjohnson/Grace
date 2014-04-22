using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This class allows you to configure a type for exporting using fluent syntax
	/// </summary>
	public partial class FluentExportStrategyConfiguration : IFluentExportStrategyConfiguration, IExportStrategyProvider
	{
		private readonly ICompiledExportStrategy exportStrategy;
		private readonly Type exportType;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType">type being exported</param>
		/// <param name="exportStrategy">configurable strategy</param>
		public FluentExportStrategyConfiguration(Type exportType, ICompiledExportStrategy exportStrategy)
		{
			this.exportType = exportType;
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			ProcessCurrentImportProperty();
			ProcessCurrentConstructorParamInfo();
			ProcessCurrentImportMethodInfo();

			yield return exportStrategy;
		}

		/// <summary>
		/// Defines the priority to export at
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Export under a particular key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration WithKey(object key)
		{
			exportStrategy.SetKey(key);

			return this;
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration As(Type exportType)
		{
			if (!this.exportType.GetTypeInfo().IsGenericTypeDefinition &&
				 !exportType.GetTypeInfo().IsAssignableFrom(this.exportType.GetTypeInfo()))
			{
				throw new ArgumentException(
					string.Format("Exported type {0} cannot be cast to {1}",
						this.exportType.FullName,
						exportType.FullName),
					"exportType");
			}

			exportStrategy.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration As<T>()
		{
			if (!exportType.GetTypeInfo().IsGenericTypeDefinition &&
				 !typeof(T).GetTypeInfo().IsAssignableFrom(exportType.GetTypeInfo()))
			{
				throw new ArgumentException(
					string.Format("Exported type {0} cannot be cast to {1}",
						exportType.FullName,
						typeof(T).FullName));
			}

			exportStrategy.AddExportType(typeof(T));

			return this;
		}

		/// <summary>
		/// Export the type by the interfaces it implements
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration ByInterfaces()
		{
			if (exportStrategy.ActivationType.GetTypeInfo().IsInterface)
			{
				exportStrategy.AddExportType(exportStrategy.ActivationType);
			}
			else
			{
				foreach (Type interfaceTypes in exportStrategy.ActivationType.GetTypeInfo().ImplementedInterfaces)
				{
					if (exportStrategy.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
					{
						exportStrategy.AddExportType(interfaceTypes.GetGenericTypeDefinition());
					}
					else
					{
						exportStrategy.AddExportType(interfaceTypes);
					}
				}
			}

			return this;
		}

		/// <summary>
		/// Defines which environment this export should be exported in
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export this type as a particular name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration AsName(string name)
		{
			exportStrategy.AddExportName(name.ToLowerInvariant());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration AndSingletonPerScope()
		{
			exportStrategy.SetLifestyleContainer(new SingletonPerScopeLifestyle());

			return this;
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration AndWeakSingleton()
		{
			exportStrategy.SetLifestyleContainer(new WeakSingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of calling Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration ExternallyOwned()
		{
			exportStrategy.SetExternallyOwned();

			return this;
		}

		/// <summary>
		/// Allows you to specify an import constructor
		/// </summary>
		/// <param name="constructorInfo">ConstrcutorInfo object to use during construction</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration ImportConstructor(ConstructorInfo constructorInfo)
		{
			exportStrategy.ImportConstructor(constructorInfo);

			return this;
		}

		/// <summary>
		/// Method to call when activation is done
		/// </summary>
		/// <param name="activationMethod"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration ActivationMethod(string activationMethod)
		{
			MethodInfo importMethod = null;

			foreach (MethodInfo runtimeMethod in exportType.GetRuntimeMethods())
			{
				ParameterInfo[] parameters = runtimeMethod.GetParameters();

				if (runtimeMethod.Name == activationMethod && parameters.Length == 0 ||
					 (parameters.Length == 1 && parameters[0].ParameterType == typeof(IInjectionContext)))
				{
					importMethod = runtimeMethod;
					break;
				}
			}

			if (importMethod == null)
			{
				throw new ArgumentException(string.Format("Could not find method {0} to import", activationMethod),
					"activationMethod");
			}

			exportStrategy.ActivateMethod(importMethod);

			return this;
		}

		/// <summary>
		/// Specify a custom Lifestyle container for export.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration UsingLifestyle(ILifestyle container)
		{
			exportStrategy.SetLifestyleContainer(container);

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IFluentExportStrategyConfiguration AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Marks all properties on the object for injection
		/// </summary>
		/// <param name="required"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration AutoWireProperties(bool required = true)
		{
			foreach (PropertyInfo propertyInfo in exportType.GetRuntimeProperties())
			{
				if (propertyInfo.CanWrite)
				{
					ImportPropertyInfo newImportPropertyInfo = new ImportPropertyInfo
																			 {
																				 Property = propertyInfo,
																				 IsRequired = required
																			 };

					exportStrategy.ImportProperty(newImportPropertyInfo);
				}
			}

			return this;
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			exportStrategy.AddCleanupDelegate(disposalCleanupDelegate);

			return this;
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			exportStrategy.EnrichWithDelegate(enrichWithDelegate);

			return this;
		}
	}

	/// <summary>
	/// This class configures a particular type for export using fluent syntax
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class FluentExportStrategyConfiguration<T> : IFluentExportStrategyConfiguration<T>,
		IExportStrategyProvider
	{
		private readonly ICompiledExportStrategy exportStrategy;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportStrategy"></param>
		public FluentExportStrategyConfiguration(ICompiledExportStrategy exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			ProcessCurrentImportProperty();
			ProcessCurrentConstructorParamInfo();
			ProcessCurrentImportMethodInfo();

			yield return exportStrategy;
		}

		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of having to call Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration<T> ExternallyOwned()
		{
			exportStrategy.SetExternallyOwned();

			return this;
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> As<TExportType>()
		{
			Type asType = typeof(TExportType);

			if (!asType.GetTypeInfo().IsGenericTypeDefinition &&
				 !asType.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new ArgumentException(
					string.Format("Exported type {0} cannot be cast to {1}",
						typeof(T).FullName,
						asType.FullName));
			}

			exportStrategy.AddExportType(asType);

			return this;
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> As(Type exportType)
		{
			if (!exportType.GetTypeInfo().IsGenericTypeDefinition &&
				 !exportType.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new ArgumentException(
					string.Format("Exported type {0} cannot be cast to {1}",
						typeof(T).FullName,
						exportType.FullName),
					"exportType");
			}

			exportStrategy.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export the type by the interfaces it implements
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ByInterfaces()
		{
			if (exportStrategy.ActivationType.GetTypeInfo().IsInterface)
			{
				exportStrategy.AddExportType(exportStrategy.ActivationType);
			}
			else
			{
				foreach (Type interfaceTypes in exportStrategy.ActivationType.GetTypeInfo().ImplementedInterfaces)
				{
					if (exportStrategy.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
					{
						exportStrategy.AddExportType(interfaceTypes.GetGenericTypeDefinition());
					}
					else
					{
						exportStrategy.AddExportType(interfaceTypes);
					}
				}
			}

			return this;
		}

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AsName(string name)
		{
			exportStrategy.AddExportName(name.ToLowerInvariant());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndSingletonPerScope()
		{
			exportStrategy.SetLifestyleContainer(new SingletonPerScopeLifestyle());

			return this;
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndWeakSingleton()
		{
			exportStrategy.SetLifestyleContainer(new WeakSingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export under a specific key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithKey(object key)
		{
			exportStrategy.SetKey(key);

			return this;
		}

		/// <summary>
		/// This method allows you to specify which constructor to use ( x => new MyTypeName("Specific","Constructor") )
		/// </summary>
		/// <param name="constructorExpression">constructor expression ( x => new MyTypeName("Specific","Constructor") )</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ImportConstructor(Expression<Action> constructorExpression)
		{
			NewExpression newExpression = constructorExpression.Body as NewExpression;

			if (newExpression != null)
			{
				exportStrategy.ImportConstructor(newExpression.Constructor);
			}

			return this;
		}

		/// <summary>
		/// Mark a particular Action() as the activation action
		/// </summary>
		/// <param name="activationMethod"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ActivationMethod(Expression<Action<T>> activationMethod)
		{
			MethodCallExpression callExpression = activationMethod.Body as MethodCallExpression;

			if (callExpression != null)
			{
				exportStrategy.ActivateMethod(callExpression.Method);
			}

			return this;
		}

		/// <summary>
		/// Specify a custom life cycle container for the export
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> UsingLifestyleContainer(ILifestyle container)
		{
			exportStrategy.SetLifestyleContainer(container);

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IFluentExportStrategyConfiguration<T> AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenInjectedInto<TInjected>()
		{
			exportStrategy.AddCondition(new WhenInjectedInto(typeof(TInjected)));

			return this;
		}

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenClassHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenClassHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenMemberHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenMemberHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenTargetHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenTargetHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Sets up all public writable properties on the type to be injected
		/// </summary>
		/// <param name="required">are the properties required</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AutoWireProperties(bool required = false)
		{
			foreach (PropertyInfo propertyInfo in typeof(T).GetRuntimeProperties())
			{
				if (propertyInfo.CanWrite && !propertyInfo.SetMethod.IsStatic)
				{
					ImportPropertyInfo newImportPropertyInfo = new ImportPropertyInfo
																			 {
																				 IsRequired = required,
																				 Property = propertyInfo
																			 };

					exportStrategy.ImportProperty(newImportPropertyInfo);
				}
			}

			return this;
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			exportStrategy.AddCleanupDelegate(disposalCleanupDelegate);

			return this;
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			exportStrategy.EnrichWithDelegate(enrichWithDelegate);

			return this;
		}
	}
}
