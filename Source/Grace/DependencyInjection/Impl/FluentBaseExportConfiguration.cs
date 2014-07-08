using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Base export strategy configuration
	/// </summary>
	public class FluentBaseExportConfiguration : IFluentExportStrategyConfiguration
	{
		protected IFluentExportStrategyConfiguration strategy;

		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="strategy"></param>
		protected FluentBaseExportConfiguration(IFluentExportStrategyConfiguration strategy)
		{
			this.strategy = strategy;
		}

		#region IFluentExportStrategy implementation

		/// <summary>
		/// Defines the priority to export at
		/// </summary>
		/// <param name="priority">priority for export</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration WithPriority(int priority)
		{
			return strategy.WithPriority(priority);
		}

		/// <summary>
		/// Export under a particular key
		/// </summary>
		/// <param name="key">key to associate with export</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration WithKey(object key)
		{
			return strategy.WithKey(key);
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <param name="exportType">type to export as</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration As(Type exportType)
		{
			return strategy.As(exportType);
		}

		/// <summary>
		/// Export the type by it's interfaces
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration ByInterfaces()
		{
			return strategy.ByInterfaces();
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration As<T>()
		{
			return strategy.As<T>();
		}

		/// <summary>
		/// Defines which environment this export should be exported in
		/// </summary>
		/// <param name="environment"></param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration InEnvironment(ExportEnvironment environment)
		{
			return strategy.InEnvironment(environment);
		}

		/// <summary>
		/// Export in a new context
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration InNewContext()
		{
			return strategy.InNewContext();
		}

	    /// <summary>
	    /// Applies a lifestyle to an export
	    /// </summary>
	    /// <returns>configuration object</returns>
        public LifestyleConfiguration Lifestyle
	    {
	        get { return strategy.Lifestyle; }
	    }

	    /// <summary>
		/// Export this type as a particular name
		/// </summary>
		/// <param name="name"></param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AsName(string name)
		{
			return strategy.AsName(name);
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AndSingleton()
		{
			return strategy.AndSingleton();
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AndSingletonPerScope()
		{
			return strategy.AndSingletonPerScope();
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AndWeakSingleton()
		{
			return strategy.AndWeakSingleton();
		}

		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of having to call Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration ExternallyOwned()
		{
			return strategy.ExternallyOwned();
		}

	    /// <summary>
	    /// Imports all public properties and methods that are attributed
	    /// </summary>
	    /// <returns>configuration object</returns>
	    public IFluentExportStrategyConfiguration ImportAttributedMembers()
	    {
	        return strategy.ImportAttributedMembers();
	    }

	    /// <summary>
		/// Allows you to specify an import constructor
		/// </summary>
		/// <param name="constructorInfo">ConstrcutorInfo object to use during construction</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration ImportConstructor(ConstructorInfo constructorInfo)
		{
			return strategy.ImportConstructor(constructorInfo);
		}

		/// <summary>
		/// Mark a property for import and specify if its required
		/// </summary>
		public IFluentImportPropertyConfiguration ImportProperty(string propertyName)
		{
			return strategy.ImportProperty(propertyName);
		}

		/// <summary>
		/// Mark a property for import and specify if its required
		/// </summary>
		/// <param name="methodName">name of method to import</param>
		/// <returns>configuration object</returns>
		public IFluentImportMethodConfiguration ImportMethod(string methodName)
		{
			return strategy.ImportMethod(methodName);
		}

		/// <summary>
		/// Export a specific property under a particular name
		/// </summary>
		/// <param name="propertyName">name of property to export</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration ExportProperty(string propertyName)
		{
			return strategy.ExportProperty(propertyName);
		}

		/// <summary>
		/// Method to call when activation is done
		/// </summary>
		/// <param name="activationMethod">name of method to activate</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration ActivationMethod(string activationMethod)
		{
			return strategy.ActivationMethod(activationMethod);
		}

		/// <summary>
		/// Specify a custom Lifestyle container for export.
		/// </summary>
		/// <param name="container">Lifestyle container for the export</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration UsingLifestyle(ILifestyle container)
		{
			return strategy.UsingLifestyle(container);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration When(ExportConditionDelegate conditionDelegate)
		{
			return strategy.When(conditionDelegate);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration Unless(ExportConditionDelegate conditionDelegate)
		{
			return strategy.Unless(conditionDelegate);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition">condition for export</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AndCondition(IExportCondition condition)
		{
			return strategy.AndCondition(condition);
		}

		/// <summary>
		/// Marks all properties on the object for injection
		/// Note: Only public writeable properties will be imported
		/// </summary>
		/// <param name="required">are all the properties required</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration AutoWireProperties(bool required = false)
		{
			return strategy.AutoWireProperties(required);
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName">metadata name</param>
		/// <param name="metadataValue">metadata value</param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration WithMetadata(string metadataName, object metadataValue)
		{
			return strategy.WithMetadata(metadataName, metadataValue);
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			return strategy.DisposalCleanupDelegate(disposalCleanupDelegate);
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			return strategy.EnrichWith(enrichWithDelegate);
		}

		/// <summary>
		/// Using this method you can add your own linq expressions to the creation process
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider)
		{
			return strategy.EnrichWithExpression(provider);
		}

		/// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(T) value for the parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorConfiguration WithCtorParam<TParam>(Func<TParam> paramValue = null)
		{
			return strategy.WithCtorParam(paramValue);
		}

	    /// <summary>
	    /// Add a specific value for a particuar parameter in the constructor
	    /// </summary>
	    /// <param name="parameterType">type of parameter</param>
	    /// <param name="paramValue">Func(T) value for the parameter</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration WithCtorParam(Type parameterType, Func<object> paramValue = null)
	    {
	        return strategy.WithCtorParam(parameterType, paramValue);
	    }

	    /// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorConfiguration WithCtorParam<TParam>(
			Func<IInjectionScope, IInjectionContext, TParam> paramValue = null)
		{
			return strategy.WithCtorParam(paramValue);
		}

	    /// <summary>
	    /// Add a specific value for a particuar parameter in the constructor
	    /// </summary>
	    /// <param name="parameterType"></param>
	    /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration WithCtorParam(Type parameterType, Func<IInjectionScope, IInjectionContext, object> paramValue)
	    {
	        return strategy.WithCtorParam(parameterType, paramValue);
	    }

	    /// <summary>
		/// Adds a constructor param of type TParam to the constructor
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <typeparam name="TItem"></typeparam>
		/// <returns></returns>
		public IFluentWithCtorCollectionConfiguration<TItem> WithCtorParamCollection<TParam, TItem>()
			where TParam : IEnumerable<TItem>
		{
			return strategy.WithCtorParamCollection<TParam, TItem>();
		}

		#endregion
	}

	/// <summary>
	/// Base export strategy configuration
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FluentBaseExportConfiguration<T> : IFluentExportStrategyConfiguration<T>
	{
		private readonly IFluentExportStrategyConfiguration<T> strategy;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="strategy"></param>
		protected FluentBaseExportConfiguration(IFluentExportStrategyConfiguration<T> strategy)
		{
			this.strategy = strategy;
		}

		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithPriority(int priority)
		{
			return strategy.WithPriority(priority);
		}

		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of having to call Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentExportStrategyConfiguration<T> ExternallyOwned()
		{
			return strategy.ExternallyOwned();
		}

	    /// <summary>
	    /// Imports all public properties and methods that are attributed
	    /// </summary>
	    /// <returns>configuration object</returns>
	    public IFluentExportStrategyConfiguration<T> ImportAttributedMembers()
	    {
	        return strategy.ImportAttributedMembers();
	    }

	    /// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> As<TExportType>()
		{
			return strategy.As<TExportType>();
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> As(Type exportType)
		{
			return strategy.As(exportType);
		}

		/// <summary>
		/// Export the type by the interfaces it implements
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ByInterfaces()
		{
			return strategy.ByInterfaces();
		}

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			return strategy.InEnvironment(environment);
		}

		/// <summary>
		/// Export in new injection context
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> InNewContext()
		{
			return strategy.InNewContext();
		}

	    /// <summary>
	    /// Applies a lifestyle to an export
	    /// </summary>
	    /// <returns>configuration object</returns>
	    public LifestyleConfiguration<T> Lifestyle
	    {
	        get { return strategy.Lifestyle; }
	    }

	    /// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AsName(string name)
		{
			return strategy.AsName(name);
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndSingleton()
		{
			return strategy.AndSingleton();
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndSingletonPerScope()
		{
			return strategy.AndSingletonPerScope();
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AndWeakSingleton()
		{
			return strategy.AndWeakSingleton();
		}

	    /// <summary>
	    /// Add a vlue to be used for constructor parameter
	    /// </summary>
	    /// <param name="parameterType">parameter type</param>
	    /// <param name="paramValue">parameter value</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration<T> WithCtorParam(Type parameterType, Func<IInjectionScope, IInjectionContext, object> paramValue)
	    {
	        return strategy.WithCtorParam(parameterType, paramValue);
	    }

	    /// <summary>
		/// Attach a key to the export
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithKey(object key)
		{
			return strategy.WithKey(key);
		}

		/// <summary>
		/// This method allows you to specify which constructor to use ( () => new MyTypeName("Specific", "Constructor") )
		/// </summary>
		/// <param name="constructorExpression">constructor expression ( () => new MyTypeName("Specific", "Constructor") )</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ImportConstructor(Expression<Action> constructorExpression)
		{
			return strategy.ImportConstructor(constructorExpression);
		}

		/// <summary>
		/// Mark a property for Import (using dependency injection container)
		/// </summary>
		/// <returns></returns>
		public IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property)
		{
			return strategy.ImportProperty(property);
		}

		/// <summary>
		/// Import a property as a collection allowing for you to specify the sort order for the import
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		public IFluentImportPropertyCollectionConfiguration<T, TItem> ImportCollectionProperty<TItem>(
			Expression<Func<T, IEnumerable<TItem>>> property)
		{
			return strategy.ImportCollectionProperty(property);
		}

		/// <summary>
		/// Mark a method to be called upon activation passing in an arguement that has be located using the IoC
		/// </summary>
		/// <param name="method">method to import</param>
		/// <returns></returns>
		public IFluentImportMethodConfiguration<T> ImportMethod(Expression<Action<T>> method)
		{
			return strategy.ImportMethod(method);
		}

		/// <summary>
		/// Export a property to be imported by other exports
		/// </summary>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		public IFluentExportPropertyConfiguration<T, TProp> ExportProperty<TProp>(Expression<Func<T, TProp>> property)
		{
			return strategy.ExportProperty(property);
		}

		/// <summary>
		/// Mark a particular Action() as the activation action
		/// </summary>
		/// <param name="activationMethod"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> ActivationMethod(Expression<Action<T>> activationMethod)
		{
			return strategy.ActivationMethod(activationMethod);
		}

		/// <summary>
		/// Specify a custom life cycle container for the export
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> UsingLifestyle(ILifestyle container)
		{
			return strategy.UsingLifestyle(container);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			return strategy.When(conditionDelegate);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			return strategy.Unless(conditionDelegate);
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IFluentExportStrategyConfiguration<T> AndCondition(IExportCondition condition)
		{
			return strategy.AndCondition(condition);
		}

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenInjectedInto<TInjected>()
		{
			return strategy.WhenInjectedInto<TInjected>();
		}

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenClassHas<TAttr>()
		{
			return strategy.WhenClassHas<TAttr>();
		}

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenMemberHas<TAttr>()
		{
			return strategy.WhenMemberHas<TAttr>();
		}

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WhenTargetHas<TAttr>()
		{
			return strategy.WhenTargetHas<TAttr>();
		}

		/// <summary>
		/// Sets up all public writable properties on the type to be injected
		/// </summary>
		/// <param name="required">are the properties required</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> AutoWireProperties(bool required = false)
		{
			return strategy.AutoWireProperties(required);
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			return strategy.WithMetadata(metadataName, metadataValue);
		}

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(TParam) for the parameter</param>
		/// <returns></returns>
		public IFluentWithCtorConfiguration<T> WithCtorParam<TParam>(Func<TParam> paramValue = null)
		{
			return strategy.WithCtorParam(paramValue);
		}

	    /// <summary>
	    /// Add a vlue to be used for constructor parameter
	    /// </summary>
	    /// <param name="parameterType">parameter type</param>
	    /// <param name="paramValue">parameter value</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration<T> WithCtorParam(Type parameterType, Func<object> paramValue = null)
	    {
	        return strategy.WithCtorParam(parameterType, paramValue);
	    }

	    /// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorConfiguration<T> WithCtorParam<TParam>(
			Func<IInjectionScope, IInjectionContext, TParam> paramValue = null)
		{
			return strategy.WithCtorParam(paramValue);
		}

		/// <summary>
		/// Import a collection allowing you to specify a filter and a sort order
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <typeparam name="TItem"></typeparam>
		/// <returns></returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>()
			where TParam : IEnumerable<TItem>
		{
			return strategy.WithCtorCollectionParam<TParam, TItem>();
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			return strategy.DisposalCleanupDelegate(disposalCleanupDelegate);
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			return strategy.EnrichWith(enrichWithDelegate);
		}

		/// <summary>
		/// USing this method you can add custom Linq Expressions to the expression tree 
		/// </summary>
		/// <param name="provider">provider class</param>
		/// <returns></returns>
		public IFluentExportStrategyConfiguration<T> EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider)
		{
			return strategy.EnrichWithExpression(provider);
		}
	}
}