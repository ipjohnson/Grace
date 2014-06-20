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
        /// Filters exports down to ones that have particular metadata
        /// </summary>
        /// <param name="metadataName">metadata name</param>
        /// <param name="metadataValue">metadata value optional</param>
        /// <returns>export configuration object</returns>
	    public static ExportsThatConfiguration HaveMetadata(string metadataName, object metadataValue = null)
	    {
	        return new ExportsThatConfiguration().HaveMetadata(metadataName,metadataValue);
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
        /// Provide a Type filter for the exports attribute
        /// </summary>
        /// <param name="consider">type filter (TypesThat will work here)</param>
        /// <returns>export configuration</returns>
	    public static ExportsThatConfiguration HaveAttribute(Func<Type, bool> consider)
	    {
	        return new ExportsThatConfiguration().HaveAttribute(consider);
	    }

		/// <summary>
		/// Creates a new type filter method that returns true if the Name of the type starts with name
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
        [Obsolete("Please use ExportsThat.Activate(TypesThat.StartWith")]
		public static ExportsThatConfiguration StartWith(string name)
		{
			return new ExportsThatConfiguration().StartWith(name);
		}

		/// <summary>
		/// Creates a new type filter that returns true if the Name ends with the provided string
		/// </summary>
		/// <param name="name">string to compare Type name to</param>
		/// <returns>export configuration object</returns>
        [Obsolete("Please use ExportsThat.Activate(TypesThat.EndWith")]
		public static ExportsThatConfiguration EndWith(string name)
		{
			return new ExportsThatConfiguration().EndWith(name);
		}

        /// <summary>
        /// Matches exports that activate a particular type
        /// </summary>
        /// <param name="activateTypeFilter">type filter (TypesThat will work here)</param>
        /// <returns>export configuration object</returns>
	    public static ExportsThatConfiguration Activate(Func<Type, bool> activateTypeFilter)
	    {
	        return new ExportsThatConfiguration();
	    }

		/// <summary>
		/// Creates a new type filter based on the types namespace
		/// </summary>
		/// <param name="namespace">namespace the type should be in</param>
		/// <param name="includeSubnamespaces">include sub namespaces</param>
		/// <returns>export configuration object</returns>
        [Obsolete("Please use ExportsThat.Activate(TypesThat.AreInTheSameNamespaceAs")]
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
        [Obsolete("Please use ExportsThat.Activate(TypesThat.AreInTheSameNamespaceAs")]
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
        [Obsolete("Please use ExportsThat.Activate(TypesThat.AreInTheSameNamespaceAs")]
		public static ExportsThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
		{
			return new ExportsThatConfiguration().AreInTheSameNamespaceAs(typeof(T), includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new Filter that selects only exports that export as a specific interface
		/// </summary>
		/// <typeparam name="T">export type</typeparam>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration AreExportedAs<T>()
		{
			return new ExportsThatConfiguration().AreExportedAs(typeof(T));
		}

		/// <summary>
		/// Creates a new Filter that selects only exports that export as a specific interface
		/// </summary>
		/// <param name="exportType">export type</param>
		/// <returns>export configuration object</returns>
		public static ExportsThatConfiguration AreExportedAs(Type exportType)
		{
			return new ExportsThatConfiguration().AreExportedAs(exportType);
		}

        /// <summary>
        /// Provide a filter that will loop through the list of export types for an export strategy
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
	    public static ExportsThatConfiguration AreExportedAs(Func<Type, bool> consider)
	    {
            return new ExportsThatConfiguration().AreExportedAs(consider);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exportName"></param>
        /// <returns></returns>
	    public static ExportsThatConfiguration AreExportedAsName(string exportName)
	    {
	        return new ExportsThatConfiguration().AreExportedAsName(exportName);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exportNameFilter"></param>
        /// <returns></returns>
	    public static ExportsThatConfiguration AreExportedAsName(Func<string, bool> exportNameFilter)
	    {
	        return new ExportsThatConfiguration().AreExportedAsName(exportNameFilter);
	    }

        /// <summary>
        /// Creates a new filter that returns true when the provided filter matches
        /// </summary>
        /// <param name="exportFilter">export filter</param>
        /// <returns>export configuration object</returns>
	    public static ExportsThatConfiguration Match(ExportStrategyFilter exportFilter)
	    {
	        return new ExportsThatConfiguration().Match(exportFilter);
	    }
	}
}