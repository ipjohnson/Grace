using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
    /// <summary>
    /// This class compiles a delegate to be used for exporting. 
    /// It creates a new instance using the types constructor
    /// </summary>
    public class InstanceCompiledExportDelegate : BaseCompiledExportDelegate
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="exportDelegateInfo"></param>
        /// <param name="exportStrategy"></param>
        /// <param name="owningScope"></param>
        public InstanceCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo,
            IExportStrategy exportStrategy,
            IInjectionScope owningScope)
            : base(exportDelegateInfo, exportStrategy, owningScope)
        {
        }

        protected override void CreateInstantiationExpression()
        {
            var context = new CustomConstructorEnrichmentLinqExpressionContext(this);

            if (this.exportDelegateInfo.CustomConstructorEnrichment == null)
            {
                ConstructorInfo constructorInfo;
                IEnumerable<Expression> parameterExpressions;

                var constructExpression = context.GetConstructorExpression(out constructorInfo, out parameterExpressions);

                instanceExpressions.Add(constructExpression);
            }
            else
            {
                var constructExpression = 
                    exportDelegateInfo.CustomConstructorEnrichment.ProvideConstructorExpressions(
                        new CustomConstructorEnrichmentLinqExpressionContext(this));

                instanceExpressions.AddRange(constructExpression);
            }
        }


        /// <summary>
        /// Picks the specific constructor to use for exporting
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        protected virtual ConstructorInfo PickConstructor(Type activationType)
        {
            return activationType.GetTypeInfo().DeclaredConstructors.
                                                            Where(x => x.IsPublic && !x.IsStatic).
                                                            OrderByDescending(x => x.GetParameters().Count()).
                                                            FirstOrDefault();
        }

        private class CustomConstructorEnrichmentLinqExpressionContext : ICustomConstructorEnrichmentLinqExpressionContext
        {
            private readonly InstanceCompiledExportDelegate _instanceCompiledExport;

            public CustomConstructorEnrichmentLinqExpressionContext(
                InstanceCompiledExportDelegate instanceCompiledExport)
            {
                _instanceCompiledExport = instanceCompiledExport;
            }

            public Type ActivationType
            {
                get { return _instanceCompiledExport.exportDelegateInfo.ActivationType; }
            }

            public ParameterExpression ExportStrategyScopeParameter
            {
                get
                {
                    return _instanceCompiledExport.exportStrategyScopeParameter;
                }
            }

            public ParameterExpression InjectionContextParameter
            {
                get
                {
                    return _instanceCompiledExport.injectionContextParameter;
                }
            }

            public ParameterExpression InstanceVariable
            {
                get { return _instanceCompiledExport.instanceVariable; }
            }

            public void AddLocalVariable(ParameterExpression newLocalVariable)
            {
                throw new NotImplementedException();
            }

            public Expression GetConstructorExpression(out ConstructorInfo constructorInfo, out IEnumerable<Expression> constructorParameters)
            {
                CompiledExportDelegateInfo exportDelegateInfo = _instanceCompiledExport.exportDelegateInfo;

                constructorInfo = _instanceCompiledExport.exportDelegateInfo.ImportConstructor ??
                                  _instanceCompiledExport.PickConstructor(exportDelegateInfo.ActivationType);

                if (constructorInfo == null)
                {
                    throw new PublicConstructorNotFoundException(exportDelegateInfo.ActivationType);
                }

                List<Expression> parameters = new List<Expression>();
                constructorParameters = parameters;

                Attribute[] constructorAttributes = constructorInfo.GetCustomAttributes(true).ToArray();

                if (constructorAttributes.Length == 0)
                {
                    constructorAttributes = EmptyAttributesArray;
                }

                foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
                {
                    Attribute[] parameterAttributes = parameterInfo.GetCustomAttributes(true).ToArray();

                    if (parameterAttributes.Length == 0)
                    {
                        parameterAttributes = EmptyAttributesArray;
                    }

                    IExportValueProvider valueProvider = null;
                    ExportStrategyFilter exportStrategyFilter = null;
                    string importName = null;
                    object comparerObject = null;
                    ILocateKeyValueProvider locateKey = null;

                    if (exportDelegateInfo.ConstructorParams != null)
                    {
                        foreach (ConstructorParamInfo constructorParamInfo in exportDelegateInfo.ConstructorParams)
                        {
                            if (string.Compare(parameterInfo.Name,
                                constructorParamInfo.ParameterName,
                                StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                importName = constructorParamInfo.ImportName;
                                exportStrategyFilter = constructorParamInfo.ExportStrategyFilter;
                                valueProvider = constructorParamInfo.ValueProvider;
                                comparerObject = constructorParamInfo.ComparerObject;
                                locateKey = constructorParamInfo.LocateKeyProvider;
                                break;
                            }
                        }

                        if (valueProvider == null)
                        {
                            foreach (ConstructorParamInfo constructorParamInfo in exportDelegateInfo.ConstructorParams)
                            {
                                if (string.IsNullOrEmpty(constructorParamInfo.ParameterName) &&
                                    parameterInfo.ParameterType.GetTypeInfo().IsAssignableFrom(
                                        constructorParamInfo.ParameterType.GetTypeInfo()))
                                {
                                    importName = constructorParamInfo.ImportName;
                                    exportStrategyFilter = constructorParamInfo.ExportStrategyFilter;
                                    valueProvider = constructorParamInfo.ValueProvider;
                                    comparerObject = constructorParamInfo.ComparerObject;
                                    locateKey = constructorParamInfo.LocateKeyProvider;
                                    break;
                                }
                            }
                        }
                    }

                    InjectionTargetInfo targetInfo = null;

                    if (importName != null)
                    {
                        targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
                            _instanceCompiledExport.activationTypeAttributes,
                            parameterInfo,
                            parameterAttributes,
                            constructorAttributes,
                            importName,
                            null);
                    }
                    else if (InjectionKernel.ImportTypeByName(parameterInfo.ParameterType))
                    {
                        targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
                            _instanceCompiledExport.activationTypeAttributes,
                            parameterInfo,
                            parameterAttributes,
                            constructorAttributes,
                            parameterInfo.Name,
                            null);
                    }
                    else
                    {
                        targetInfo = new InjectionTargetInfo(exportDelegateInfo.ActivationType,
                            _instanceCompiledExport.activationTypeAttributes,
                            parameterInfo,
                            parameterAttributes,
                            constructorAttributes,
                            null,
                            parameterInfo.ParameterType);
                    }

                    ParameterExpression parameterExpression =
                            _instanceCompiledExport.CreateImportExpression(parameterInfo.ParameterType,
                            targetInfo,
                            ExportStrategyDependencyType.ConstructorParameter,
                            importName,
                            parameterInfo.Name + "CVar",
                            true,
                            valueProvider,
                            exportStrategyFilter,
                            locateKey,
                            comparerObject,
                            null);

                    parameters.Add(Expression.Convert(parameterExpression, parameterInfo.ParameterType));
                }

                Expression constructExpression = Expression.New(constructorInfo, parameters.ToArray());

                return Expression.Assign(_instanceCompiledExport.instanceVariable, constructExpression);
            }


            public ParameterExpression GetLocateDependencyExpression(Type importType = null,
                string importName = null,
                IInjectionTargetInfo targetInfo = null,
                string variableName = null,
                bool isRequired = true,
                IExportValueProvider valueProvider = null,
                ExportStrategyFilter exportStrategyFilter = null,
                ILocateKeyValueProvider locateKey = null,
                object comparerObject = null)
            {
                if (importType == null && importName == null)
                {
                    throw new ArgumentException("GetLocateDependencyExpression must be called with importType or ");
                }

                return null;
            }
        }
    }
}