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
		/// WithCtorParam(() => someLocalVariable).Named("someLocalVariable") is the long hand form
		/// </summary>
		/// <typeparam name="TValue">value type being used</typeparam>
		/// <param name="strategy">export strategy</param>
		/// <param name="valueExpression">value expression, the name of the parameter will be used as the parameter name</param>
		/// <returns>configuration object</returns>
		public static IFluentWithCtorConfiguration WithNamedCtorValue<TValue>(this IFluentExportStrategyConfiguration strategy, Expression<Func<TValue>> valueExpression)
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

			throw new Exception("WithNamedCtorValue must be passed a Func that references a member");
		}

		/// <summary>
		/// This is intended to be a short cut for setting named property values
		/// The expression will be inspected and the value will used by the property name
		/// WithNameCtorValue(() => someLocalVariable) will export the value under the name someLocalVariable
		/// </summary>
		/// <typeparam name="T">Type being exported</typeparam>
		/// <typeparam name="TValue">value type being used</typeparam>
		/// <param name="strategy">export strategy</param>
		/// <param name="valueExpression">value expression, the name of the parameter will be used as the parameter name</param>
		/// <returns>configuration object</returns>
		public static IFluentWithCtorConfiguration<T> WithNamedCtorValue<T,TValue>(this IFluentExportStrategyConfiguration<T> strategy, Expression<Func<TValue>> valueExpression)
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

			throw new Exception("WithNamedCtorValue must be passed a Func that references a member");
		}

		#endregion
    }
}
