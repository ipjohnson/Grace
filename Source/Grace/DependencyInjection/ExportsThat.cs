using System;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// static class that provides ExportStrategyFilter methods to be used during export registration
	/// </summary>
	public static class ExportsThat
	{
		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <param name="attributeType">attribute type</param>
		/// <param name="attributeFilter">attribute filter func</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration HaveAttribute(Type attributeType, Func<Attribute, bool> attributeFilter = null)
		{
			return new ExportsThatConfiguration().HaveAttribute(attributeType, attributeFilter);
		}

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <typeparam name="TAttribute">attribute type</typeparam>
		/// <param name="attributeFilter">attribute filter func</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
			where TAttribute : Attribute
		{
			return new ExportsThatConfiguration().HaveAttribute(attributeFilter);
		}

		/// <summary>
		/// Creates a new type filter method that returns true if the Name of the type starts with name
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration StartWith(string name)
		{
			return new ExportsThatConfiguration().StartWith(name);
		}

		/// <summary>
		/// Creates a new type filter that returns true if the Name ends with the provided string
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration EndWith(string name)
		{
			return new ExportsThatConfiguration().EndWith(name);
		}

		/// <summary>
		/// Creates a new type filter based on the types namespace
		/// </summary>
		/// <param name="namespace">namespace the type should be in</param>
		/// <param name="includeSubnamespaces">include sub namespaces</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
		{
			return new ExportsThatConfiguration().AreInTheSameNamespace(@namespace, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <param name="type">class to check for</param>
		/// <param name="includeSubnamespaces">include sub namespaces</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
		{
			return new ExportsThatConfiguration().AreInTheSameNamespace(type.Namespace, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <typeparam name="T">class to check for</typeparam>
		/// <param name="includeSubnamespaces">include sub namespace</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
		{
			return new ExportsThatConfiguration().AreInTheSameNamespaceAs(typeof(T), includeSubnamespaces);
		}
	}
}