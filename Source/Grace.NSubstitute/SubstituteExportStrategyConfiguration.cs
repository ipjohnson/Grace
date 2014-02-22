using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.NSubstitute
{
	public class SubstituteExportStrategyConfiguration<T> : ISubstituteExportStrategyConfiguration<T>
	{
		private readonly ICompiledExportStrategy exportStrategy;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportStrategy"></param>
		public SubstituteExportStrategyConfiguration(ICompiledExportStrategy exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> As<TExportType>()
		{
			exportStrategy.AddExportType(typeof(TExportType));

			return this;
		}

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> AsName(string name)
		{
			exportStrategy.AddExportName(name);

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> AndSingletonPerScope()
		{
			exportStrategy.SetLifestyleContainer(new SingletonPerScopeLifestyle());

			return this;
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> AndWeakSingleton()
		{
			exportStrategy.SetLifestyleContainer(new WeakSingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export under a specific key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WithKey(object key)
		{
			exportStrategy.SetKey(key);

			return this;
		}

		/// <summary>
		/// Specify a custom life cycle container for the export
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> UsingLifestyleContainer(ILifestyle container)
		{
			exportStrategy.SetLifestyleContainer(container);

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public ISubstituteExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public ISubstituteExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public ISubstituteExportStrategyConfiguration<T> AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WhenInjectedInto<TInjected>()
		{
			exportStrategy.AddCondition(new WhenInjectedInto(typeof(TInjected)));

			return this;
		}

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WhenClassHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenClassHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WhenMemberHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenMemberHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WhenTargetHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenTargetHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Sets up all public writable properties on the type to be injected
		/// </summary>
		/// <param name="required">are the properties required</param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> AutoWireProperties(bool required = false)
		{
			foreach (PropertyInfo propertyInfo in typeof(T).GetRuntimeProperties())
			{
				if (propertyInfo.CanWrite)
				{
					// Fix Me
					//exportStrategy.ImportProperty(propertyInfo, null, required, null, null, null);
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
		public ISubstituteExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">value for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(TParam paramValue, string paramName = null)
		{
			return WithCtorParam<TParam>(new FuncValueProvider<TParam>(() => paramValue), paramName);
		}

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(TParam) for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(Func<TParam> paramValue,
			string paramName = null)
		{
			return WithCtorParam<TParam>(new FuncValueProvider<TParam>(paramValue), paramName);
		}

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">value provider for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(IExportValueProvider paramValue,
			string paramName = null,
			ExportStrategyFilter consider = null)
		{
			// Fix Me
			// exportStrategy.WithCtorParam(paramValue, paramName, typeof(TParam), consider, null);

			return this;
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns></returns>
		public ISubstituteExportStrategyConfiguration<T> EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			exportStrategy.EnrichWithDelegate(enrichWithDelegate);

			return this;
		}

		public ISubstituteExportStrategyConfiguration<T> Arrange(Action<T> setupAction)
		{
			exportStrategy.EnrichWithDelegate((scope, context, injected) =>
			                                  {
				                                  setupAction((T)injected);

				                                  return injected;
			                                  });

			return this;
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			exportStrategy.Initialize();

			yield return exportStrategy;
		}
	}
}