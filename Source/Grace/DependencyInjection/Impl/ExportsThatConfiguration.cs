using System;
using System.Collections.Generic;
using System.Linq;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Configuration for ExportsThat, build a 
	/// </summary>
	public class ExportsThatConfiguration
	{
		private readonly List<ExportStrategyFilter> exportStrategyFilters = new List<ExportStrategyFilter>();

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <param name="attributeType">attribute type</param>
		/// <param name="attributeFilter">attribute filter func</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration HaveAttribute(Type attributeType, Func<Attribute, bool> attributeFilter = null)
		{
			if (attributeFilter != null)
			{
				exportStrategyFilters.Add((context, strategy) => strategy.Attributes.Any(attributeFilter));
			}
			else
			{
				exportStrategyFilters.Add((context, strategy) => strategy.Attributes.Any());
			}

			return this;
		}

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <typeparam name="TAttribute">attribute type</typeparam>
		/// <param name="attributeFilter">attribute filter func</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
			where TAttribute : Attribute
		{
			if (attributeFilter != null)
			{
				exportStrategyFilters.Add((context, strategy) => strategy.Attributes.Any(
					x =>
					{
						bool returnValue = false;
						TAttribute attribute =
							x as TAttribute;

						if (attribute != null)
						{
							returnValue = attributeFilter(attribute);
						}

						return returnValue;
					}));
			}
			else
			{
				exportStrategyFilters.Add((context, strategy) => strategy.Attributes.Any());
			}

			return this;
		}

		/// <summary>
		/// Creates a new type filter method that returns true if the Name of the type starts with name
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration StartWith(string name)
		{
			exportStrategyFilters.Add((context, strategy) => strategy.ActivationType.Name.StartsWith(name));

			return this;
		}

		/// <summary>
		/// Creates a new type filter that returns true if the Name ends with the provided string
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration EndWith(string name)
		{
			exportStrategyFilters.Add((context, strategy) => strategy.ActivationType.Name.EndsWith(name));

			return this;
		}

		/// <summary>
		/// Creates a new type filter based on the types namespace
		/// </summary>
		/// <param name="namespace">namespace the type should be in</param>
		/// <param name="includeSubnamespaces">include sub namespaces</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
		{
			if (includeSubnamespaces)
			{
				exportStrategyFilters.Add((context, strategy) => strategy.ActivationType.Namespace == @namespace ||
				                                                 strategy.ActivationType.Namespace != null &&
				                                                 strategy.ActivationType.Namespace.StartsWith(@namespace + "."));
			}
			else
			{
				exportStrategyFilters.Add((context, strategy) => strategy.ActivationType.Namespace == @namespace);
			}

			return this;
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <param name="type">class to check for</param>
		/// <param name="includeSubnamespaces">include sub namespaces</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
		{
			return AreInTheSameNamespace(type.Namespace, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <typeparam name="T">class to check for</typeparam>
		/// <param name="includeSubnamespaces">include sub namespace</param>
		/// <returns>export configuration object</returns>
		public ExportsThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
		{
			return AreInTheSameNamespaceAs(typeof(T), includeSubnamespaces);
		}

		/// <summary>
		/// Converts the configuration to a filter automatically
		/// </summary>
		/// <param name="configuration">configuration object</param>
		/// <returns>new filter method</returns>
		public static implicit operator ExportStrategyFilter(ExportsThatConfiguration configuration)
		{
			return new ExportStrategyFilterGroup(configuration.exportStrategyFilters.ToArray());
		}
	}
}