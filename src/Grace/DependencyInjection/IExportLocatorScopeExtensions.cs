using System;
using System.Text;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extension class for IExportLocatorScope
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportLocatorScopeExtensions
    {
        /// <summary>
        /// Get the parent injection scope for this export locator scope
        /// </summary>
        /// <param name="scope">export locator scope</param>
        /// <returns>parent injection scope</returns>
        public static IInjectionScope GetInjectionScope(this IExportLocatorScope scope)
        {
            while (!(scope is IInjectionScope))
            {
                scope = scope.Parent;
            }

            return (IInjectionScope)scope;
        }

        /// <summary>
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="includeParent"></param>
        /// <param name="consider"></param>
        /// <returns></returns>
        public static string WhatDoIHave(this IExportLocatorScope scope, bool includeParent = true,
            ActivationStrategyFilter consider = null)
        {

            var builder = new StringBuilder();

            var locator = scope.GetInjectionScope();

            builder.AppendLine(new string('-', 80));

            builder.AppendFormat("Exports for scope '{0}' with id {1}{2}",
                locator.ScopeName,
                locator.ScopeId,
                Environment.NewLine);

            foreach (var exportStrategy in locator.StrategyCollectionContainer.GetAllStrategies())
            {
                builder.AppendLine(new string('-', 80));

                builder.AppendLine("Export Type: " + exportStrategy.ActivationType.FullName);

                foreach (var exportType in exportStrategy.ExportAs)
                {
                    builder.AppendLine("As Type: " + exportType);
                }
                
                builder.AppendLine("Priority: " + exportStrategy.Priority);

                builder.AppendLine("Externally Owned: " + exportStrategy.ExternallyOwned);

                if (exportStrategy.Lifestyle != null)
                {
                    builder.AppendLine("Lifestyle: " + exportStrategy.Lifestyle.GetType().Name);
                }
                else
                {
                    builder.AppendLine("Lifestyle: Transient");
                }
                
                builder.AppendLine("Depends On");

                var hasDependency = false;

                foreach (var exportStrategyDependency in exportStrategy.GetDependencies())
                {
                    builder.AppendLine("\tDependency Type: " + exportStrategyDependency.DependencyType);

                    builder.AppendLine("\tMember Name: " + exportStrategyDependency.MemberName);

                    builder.AppendLine("\tImport Type: " + exportStrategyDependency.TypeBeingImported.FullName);
                    
                    builder.AppendLine("\tHas Filter: " + exportStrategyDependency.HasFilter);

                    builder.AppendLine("\tHas Value Provider: " + exportStrategyDependency.HasValueProvider);

                    builder.AppendLine("\tIs Satisfied: " + exportStrategyDependency.IsSatisfied);

                    builder.AppendLine();

                    hasDependency = true;
                }

                if (!hasDependency)
                {
                    builder.AppendLine("\tNone");
                }
            }

            if (includeParent && locator.Parent != null)
            {
                builder.Append(WhatDoIHave(scope.Parent, true, consider));
            }

            return builder.ToString();

        }
    }
}
