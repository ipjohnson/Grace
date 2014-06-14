using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration for ExportsThat, build a 
    /// </summary>
    public class ExportsThatConfiguration
    {
        private readonly List<ExportStrategyFilter> exportStrategyFilters = new List<ExportStrategyFilter>();
        private bool useOr = false;

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
                exportStrategyFilters.Add((context, strategy) =>
                    strategy.Attributes.Where(a => a.GetType().GetTypeInfo().IsAssignableFrom(attributeType.GetTypeInfo())).
                                        Any(attributeFilter));
            }
            else
            {
                exportStrategyFilters.Add((context, strategy) =>
                    strategy.Attributes.Any(a => a.GetType().GetTypeInfo().IsAssignableFrom(attributeType.GetTypeInfo())));
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
                exportStrategyFilters.Add((context, strategy) =>
                    strategy.Attributes.Where(a => a.GetType().GetTypeInfo().IsAssignableFrom(typeof(TAttribute).GetTypeInfo())).Any(
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
                exportStrategyFilters.Add((context, strategy) =>
                    strategy.Attributes.Any(a => a.GetType().GetTypeInfo().IsAssignableFrom(typeof(TAttribute).GetTypeInfo())));
            }

            return this;
        }

        /// <summary>
        /// Creates a new filter that returns true when metadata matches
        /// </summary>
        /// <param name="metadataName"></param>
        /// <param name="metadataValue"></param>
        /// <returns></returns>
        public ExportsThatConfiguration HaveMetadata(string metadataName, object metadataValue = null)
        {
            if (metadataValue == null)
            {
                exportStrategyFilters.Add((context, strategy) => strategy.Metadata.ContainsKey(metadataName));
            }
            else
            {
                object tempObject = metadataValue;

                exportStrategyFilters.Add((context, strategy) => strategy.Metadata.MetadataMatches(metadataName, tempObject));
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
        /// Creates a new filter that selects only exports that exoprt as a particular interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ExportsThatConfiguration AreExportedAs<T>()
        {
            return AreExportedAs(typeof(T));
        }

        /// <summary>
        /// Creates a new filter that selects only exports that exoprt as a particular interface
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public ExportsThatConfiguration AreExportedAs(Type exportType)
        {
            exportStrategyFilters.Add((context, strategy) => strategy.ExportTypes.Any(x => x == exportType));

            return this;
        }

        /// <summary>
        /// Creates a new filter that checks to see if it's exported type meets the criteria
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
        public ExportsThatConfiguration AreExportedAs(Func<Type, bool> consider)
        {
            exportStrategyFilters.Add((context, strategy) => strategy.ExportTypes.Any(consider));

            return this;
        }

        /// <summary>
        /// Is the strategy exported as a particular name
        /// </summary>
        /// <param name="exportName">export name</param>
        /// <returns>configuration object</returns>
        public ExportsThatConfiguration AreExportedAsName(string exportName)
        {
            exportStrategyFilters.Add((context,strategy) => strategy.ExportNames.Contains(exportName));

            return this;
        }

        /// <summary>
        /// Is the strategy exported as a particular name
        /// </summary>
        /// <param name="exportFilter">export filter</param>
        /// <returns>configuration object</returns>
        public ExportsThatConfiguration AreExportedAsName(Func<string, bool> exportFilter)
        {
            exportStrategyFilters.Add((context,strategy) => strategy.ExportNames.Any(exportFilter));

            return this;
        }

        /// <summary>
        /// Adds a export strategy filter
        /// </summary>
        /// <param name="exportFilter">export strategy filter</param>
        /// <returns>export strategy filter</returns>
        public ExportsThatConfiguration Match(ExportStrategyFilter exportFilter)
        {
            exportStrategyFilters.Add(exportFilter);

            return this;
        }

        /// <summary>
        /// Use or logic instead of and
        /// </summary>
        public ExportsThatConfiguration Or
        {
            get
            {
                useOr = true;

                return this;
            }
        }

        /// <summary>
        /// Use and logic, this is the default
        /// </summary>
        public ExportsThatConfiguration And
        {
            get
            {
                if (useOr)
                {
                    throw new Exception("And cannot be used in conjuction with Or");
                }
                useOr = false;

                return this;
            }
        }

        /// <summary>
        /// Converts the configuration to a filter automatically
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <returns>new filter method</returns>
        public static implicit operator ExportStrategyFilter(ExportsThatConfiguration configuration)
        {
            return new ExportStrategyFilterGroup(configuration.exportStrategyFilters.ToArray()) { UseOr = configuration.useOr };
        }

    }
}