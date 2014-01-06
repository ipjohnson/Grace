using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
		/// <param name="owningScope"></param>
		public InstanceCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo,
			IInjectionScope owningScope)
			: base(exportDelegateInfo, owningScope)
		{
		}

		protected override void CreateInstantiationExpression()
		{
			ConstructorInfo constructorInfo = exportDelegateInfo.ImportConstructor ??
			                                  PickConstructor(exportDelegateInfo.ActivationType);

			List<Expression> parameterExpressions = new List<Expression>();
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

				InjectionTargetInfo targetInfo =
					new InjectionTargetInfo(exportDelegateInfo.ActivationType,
						activationTypeAttributes,
						parameterInfo,
						parameterAttributes,
						constructorAttributes);

				IExportValueProvider valueProvider = null;
				ExportStrategyFilter exportStrategyFilter = null;
				object comparerObject = null;

				if (exportDelegateInfo.ConstructorParams != null)
				{
					foreach (ConstructorParamInfo constructorParamInfo in exportDelegateInfo.ConstructorParams)
					{
						if (string.Compare(parameterInfo.Name, constructorParamInfo.ParameterName, StringComparison.OrdinalIgnoreCase) ==
						    0)
						{
							exportStrategyFilter = constructorParamInfo.ExportStrategyFilter;
							valueProvider = constructorParamInfo.ValueProvider;
							comparerObject = constructorParamInfo.ComparerObject;
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
								exportStrategyFilter = constructorParamInfo.ExportStrategyFilter;
								valueProvider = constructorParamInfo.ValueProvider;
								comparerObject = constructorParamInfo.ComparerObject;
								break;
							}
						}
					}
				}

				ParameterExpression parameterExpression =
					CreateImportExpression(parameterInfo.ParameterType,
						targetInfo,
						ExportStrategyDependencyType.ConstructorParameter,
						null,
						parameterInfo.Name + "CVar",
						true,
						valueProvider,
						exportStrategyFilter,
						comparerObject);

				parameterExpressions.Add(Expression.Convert(parameterExpression, parameterInfo.ParameterType));
			}

			Expression constructExpression = Expression.New(constructorInfo, parameterExpressions.ToArray());

			instanceExpressions.Add(Expression.Assign(instanceVariable, constructExpression));
		}

		/// <summary>
		/// Picks the specific constructor to use for exporting
		/// </summary>
		/// <param name="activationType"></param>
		/// <returns></returns>
		protected virtual ConstructorInfo PickConstructor(Type activationType)
		{
			return activationType.GetTypeInfo().DeclaredConstructors.
				OrderByDescending(x => x.GetParameters().Count()).
				First();
		}
	}
}