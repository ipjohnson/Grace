using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public static class AttributeProcesing
    {
        public static void ProcessClassAttributes(this ICompiledExportStrategy strategy)
        {
            foreach(var attribute in strategy.ActivationType.GetTypeInfo().GetCustomAttributes())
            {
                ICustomEnrichmentExpressionAttribute enrichment = attribute as ICustomEnrichmentExpressionAttribute;

                if (enrichment != null)
                {
                    ICustomEnrichmentLinqExpressionProvider provider = enrichment.GetProvider(strategy.ActivationType, null);

                    if (provider != null)
                    {
                        strategy.EnrichWithExpression(provider);
                    }
                }

                IInNewContextAttribute newContextAttribute = attribute as IInNewContextAttribute;

                if (newContextAttribute != null)
                {
                    strategy.InNewContext();
                }

                IExportConditionAttribute conditionAttribute = attribute as IExportConditionAttribute;

                if(conditionAttribute != null)
                {
                    var condition = conditionAttribute.ProvideCondition(strategy.ActivationType);

                    if(condition != null)
                    {
                        strategy.AddCondition(condition);
                    }
                }

                IExportPriorityAttribute priorityAttribute = attribute as IExportPriorityAttribute;

                if(priorityAttribute != null)
                {
                    int priority = priorityAttribute.ProvidePriority(strategy.ActivationType);

                    strategy.SetPriority(priority);
                }

                IExportMetadataAttribute metaDataAttribute = attribute as IExportMetadataAttribute;

                if(metaDataAttribute != null)
                {
                    var metadata = metaDataAttribute.ProvideMetadata(strategy.ActivationType);

                    foreach(var metadataItem in metadata)
                    {
                        strategy.AddMetadata(metadataItem.Key, metadataItem.Value);
                    }
                }
            }
        }        

        public static void ProcessMemeberAttributes(this ICompiledExportStrategy strategy)
        {
            var type = strategy.ActivationType;

            foreach(var property in strategy.ActivationType.GetRuntimeProperties())
            {
                if(!property.CanWrite || 
                    property.SetMethod.IsStatic)
                {
                    continue;
                }

                List<IExportCondition> exportConditions = new List<IExportCondition>();
                List<IExportAttribute> exportAttributes = new List<IExportAttribute>();
                IImportAttribute importAttribute = null;
                object comparer = null;
                ExportStrategyFilter filter = null;

                foreach (Attribute customAttribute in property.GetCustomAttributes())
                {
                    if (customAttribute is IImportAttribute)
                    {
                        importAttribute = customAttribute as IImportAttribute;
                    }

                    IImportFilterAttribute filterAttribute = customAttribute as IImportFilterAttribute;

                    if (filterAttribute != null)
                    {
                        filter = filterAttribute.ProvideFilter(type, property.Name);
                    }

                    IImportSortCollectionAttribute sortAttribute = customAttribute as IImportSortCollectionAttribute;

                    if (sortAttribute != null)
                    {
                        comparer = sortAttribute.ProvideComparer(type, property.Name);
                    }

                    IExportAttribute exportAttribute = customAttribute as IExportAttribute;

                    if (exportAttribute != null)
                    {
                        exportAttributes.Add(exportAttribute);
                    }

                    IExportConditionAttribute conditionAttribute = customAttribute as IExportConditionAttribute;

                    if (conditionAttribute != null)
                    {
                        IExportCondition condition = conditionAttribute.ProvideCondition(property.PropertyType);

                        if (condition != null)
                        {
                            exportConditions.Add(condition);
                        }
                    }
                }

                if (importAttribute != null && property.CanWrite)
                {
                    ImportAttributeInfo attributeInfo =
                        importAttribute.ProvideImportInfo(type, property.Name);

                    if (filter == null)
                    {
                        filter = attributeInfo.ExportStrategyFilter;
                    }

                    if (comparer == null)
                    {
                        comparer = attributeInfo.Comparer;
                    }

                    if (attributeInfo.ImportKey != null)
                    {
                        ExportStrategyFilter keyFilter =
                            (context, strategyLocal) =>
                                IExportLocatorExtensions.CompareKeyFunction(attributeInfo.ImportKey, context, strategyLocal);

                        filter = filter != null
                            ? new ExportStrategyFilterGroup(keyFilter, filter)
                            : keyFilter;
                    }

                    ImportPropertyInfo importPropertyInfo = new ImportPropertyInfo
                    {
                        ComparerObject = comparer,
                        ExportStrategyFilter = filter,
                        ImportName = attributeInfo.ImportName,
                        IsRequired = attributeInfo.IsRequired,
                        Property = property,
                        ValueProvider = attributeInfo.ValueProvider
                    };

                    strategy.ImportProperty(importPropertyInfo);
                }

                if(exportAttributes.Count > 0)
                {
                    var exportTypes = new List<Type>();
                    var exportNames = new List<string>();

                    foreach(var exportAttr in exportAttributes)
                    {
                        exportTypes.AddRange(exportAttr.ProvideExportTypes(property.PropertyType));
                        exportNames.AddRange(exportAttr.ProvideExportNames(property.PropertyType));
                    }
                                        
                    strategy.ExportProperty(new ExportPropertyInfo
                    {
                        PropertyInfo = property,
                        ExportTypes = exportTypes,
                        ExportNames = exportNames,
                        ExportCondition = exportConditions.FirstOrDefault()
                    });
                }
            }

            foreach(var method in strategy.ActivationType.GetRuntimeMethods())
            {
                if(!method.IsPublic ||
                    method.IsStatic)
                {
                    continue;
                }

                foreach(var attribute in method.GetCustomAttributes())
                {
                    if (attribute is IImportAttribute)
                    {
                        ImportMethodInfo info = new ImportMethodInfo
                        {
                            MethodToImport = method
                        };

                        strategy.ImportMethod(info);
                    }
                    else if(attribute is ICustomEnrichmentExpressionAttribute)
                    {
                        ICustomEnrichmentExpressionAttribute enrichement = (ICustomEnrichmentExpressionAttribute)attribute;

                        var provider = enrichement.GetProvider(strategy.ActivationType, method);

                        if(provider != null)
                        {
                            strategy.EnrichWithExpression(provider);
                        }
                    }
                }
            }
        }
    }
}
