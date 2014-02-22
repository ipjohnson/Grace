using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	public class SimpleGenericExportStrategy : ConfigurableExportStrategy, IGenericExportStrategy
	{
		private readonly List<Type> exportAsTypes = new List<Type>(1);

		public SimpleGenericExportStrategy(Type exportType)
			: base(exportType)
		{
		}

		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks to make sure the closing types meet generic constraints
		/// </summary>
		/// <param name="closingTypes"></param>
		/// <returns></returns>
		public bool CheckGenericConstrataints(Type[] closingTypes)
		{
			Type[] exportingTypes = exportType.GetTypeInfo().GenericTypeParameters;

			if (closingTypes.Length != exportingTypes.Length)
			{
				return false;
			}

			bool constraintsMatch = true;

			for (int i = 0; i < exportingTypes.Length && constraintsMatch; i++)
			{
				Type[] constraints = exportingTypes[i].GetTypeInfo().GetGenericParameterConstraints();

				foreach (Type constraint in constraints)
				{
					if (constraint.GetTypeInfo().IsInterface)
					{
						if (
							closingTypes[i].GetTypeInfo()
								.ImplementedInterfaces.Any(x => x.GetTypeInfo().GUID == constraint.GetTypeInfo().GUID))
						{
							continue;
						}

						constraintsMatch = false;
						break;
					}

					if (!constraint.GetTypeInfo().IsAssignableFrom(closingTypes[i].GetTypeInfo()))
					{
						constraintsMatch = false;
						break;
					}
				}
			}

			return constraintsMatch;
		}

		/// <summary>
		/// Creates a new closed export strategy that can be activated
		/// </summary>
		/// <param name="closingTypes"></param>
		/// <returns></returns>
		public IExportStrategy CreateClosedStrategy(Type[] closingTypes)
		{
			try
			{
				Type closedType = exportType.MakeGenericType(closingTypes);
				TypeInfo closedTypeInfo = closedType.GetTypeInfo();
				SimpleExportStrategy newExportStrategy = new SimpleExportStrategy(closedType);

				foreach (string exportName in base.ExportNames)
				{
					newExportStrategy.AddExportName(exportName);
				}

				foreach (Type exportAsType in ExportTypes)
				{
					if (exportAsType.GetTypeInfo().IsGenericTypeDefinition)
					{
						Type closingType = exportAsType.MakeGenericType(closingTypes);

						newExportStrategy.AddExportType(closingType);
					}
					else
					{
						newExportStrategy.AddExportType(exportAsType);
					}
				}

				if (Lifestyle != null)
				{
					newExportStrategy.SetLifestyleContainer(Lifestyle.Clone());
				}

				return newExportStrategy;
			}
			catch (Exception exp)
			{
				string errorMessage = string.Format("Exception thrown while trying to close generic export {0} with ",
					exportType.FullName);

				closingTypes.Aggregate(errorMessage, (error, t) => error + t.FullName);

				Log.Error(errorMessage, exp);
			}

			return null;
		}
	}
}