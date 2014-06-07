using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Extension methods for IFluentExportStrategyConfiguration
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IFluentExportStrategyConfigurationExtensions
	{
		#region Singleton Per Injection Context
		/// <summary>
		/// Registers the export as singleton per injection context
		/// </summary>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration AndSingletonPerInjectionContext(this IFluentExportStrategyConfiguration configuration)
		{
			configuration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

			return configuration;
		}

		/// <summary>
		/// Registers the export as singleton per injection context
		/// </summary>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration<T> AndSingletonPerInjectionContext<T>(this IFluentExportStrategyConfiguration<T> configuration)
		{
			configuration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

			return configuration;
		}
		#endregion

		#region And Singleton Per Ancestor
		/// <summary>
		/// Registers the export as singleton per injection ancestor
		/// </summary>
		/// <typeparam name="TAncestor">ancestor type</typeparam>
		/// <param name="configuration">configuration object</param>
		/// <param name="metadataObject">metadata to match</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration AndSingletonPerAncestor<TAncestor>(this IFluentExportStrategyConfiguration configuration, object metadataObject = null)
		{
			configuration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor), metadataObject));

			return configuration;
		}

		/// <summary>
		/// Registers the export as singleton per injection ancestor
		/// </summary>
		/// <typeparam name="TAncestor">ancestor type</typeparam>
		/// <param name="configuration">configuration object</param>
		/// <param name="metadata">metadata to match</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration AndSingletonPerAncestor<TAncestor>(this IFluentExportStrategyConfiguration configuration, IEnumerable<KeyValuePair<string, object>> metadata)
		{
			configuration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor), metadata));

			return configuration;
		}

		/// <summary>
		/// Registers the export as singleton per injection ancestor
		/// </summary>
		/// <typeparam name="T">ancestor type</typeparam>
		/// <typeparam name="TAncestor">ancestor type</typeparam>
		/// <param name="configuration">configuration object</param>
		/// <param name="metadata">metadata to match</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration<T> AndSingletonPerAncestor<T, TAncestor>(this IFluentExportStrategyConfiguration<T> configuration, IEnumerable<KeyValuePair<string, object>> metadata)
		{
			configuration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor), metadata));

			return configuration;
		}
		#endregion

		#region singleton per request
		/// <summary>
		/// Register the export as singleton per request
		/// </summary>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration AndSingletonPerRequest(this IFluentExportStrategyConfiguration configuration)
		{
			configuration.UsingLifestyle(new SingletonPerRequestLifestyle());

			return configuration;
		}

		/// <summary>
		/// Register the export as singleton per request
		/// </summary>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration<T> AndSingletonPerRequest<T>(this IFluentExportStrategyConfiguration<T> configuration)
		{
			configuration.UsingLifestyle(new SingletonPerRequestLifestyle());

			return configuration;
		}
		#endregion

		#region When Ancestor

		/// <summary>
		/// Export strategy when being inserted into object graph that has an ancestor of type TAncestor
		/// </summary>
		/// <typeparam name="TAncestor">ancestor type</typeparam>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration WhenAncestor<TAncestor>(this IFluentExportStrategyConfiguration configuration)
		{
			configuration.AndCondition(new WhenAncestor(typeof(TAncestor)));

			return configuration;
		}

		/// <summary>
		/// Export strategy when being inserted into object graph that has an ancestor of type TAncestor
		/// </summary>
		/// <typeparam name="TAncestor">ancestor type</typeparam>
		/// <typeparam name="T">type being exported</typeparam>
		/// <param name="configuration">configuration object</param>
		/// <returns>configuration object</returns>
		public static IFluentExportStrategyConfiguration<T> WhenAncestor<T,TAncestor>(this IFluentExportStrategyConfiguration<T> configuration)
		{
			configuration.AndCondition(new WhenAncestor(typeof(TAncestor)));

			return configuration;
		}

		#endregion

		#region WithNamedCstorValue
		
		/// <summary>
		/// This is intended to be a short cut for setting named property values
		/// The expression will be inspected and the value will used by the property name
		/// WithNameCtorValue(() => someLocalVariable) will export the value under the name someLocalVariable
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="strategy"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		public static IFluentWithCtorConfiguration<TValue> WithNamedCtorValue<TValue>(this IFluentExportStrategyConfiguration strategy, Expression<Func<TValue>> valueExpression)
		{
			MemberExpression memberExpression = valueExpression.Body as MemberExpression;
			string exportName = null;

			if (memberExpression != null)
			{
				exportName = memberExpression.Member.Name;
			}
			
			if (exportName != null)
			{
				Func<TValue> func = valueExpression.Compile();

				return strategy.WithCtorParam(func).Named(memberExpression.Member.Name);
			}

			throw new Exception("Blah");
		}

		/// <summary>
		/// This is intended to be a short cut for setting named property values
		/// The expression will be inspected and the value will used by the property name
		/// WithNameCtorValue(() => someLocalVariable) will export the value under the name someLocalVariable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="strategy"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		public static IFluentWithCtorConfiguration<T,TValue> WithNamedCtorValue<T,TValue>(this IFluentExportStrategyConfiguration<T> strategy, Expression<Func<TValue>> valueExpression)
		{
			MemberExpression memberExpression = valueExpression.Body as MemberExpression;
			string exportName = null;

			if (memberExpression != null)
			{
				exportName = memberExpression.Member.Name;
			}

			if (exportName != null)
			{
				Func<TValue> func = valueExpression.Compile();

				return strategy.WithCtorParam(func).Named(memberExpression.Member.Name);
			}

			throw new Exception("Blah");
		}

		#endregion

	}
}
