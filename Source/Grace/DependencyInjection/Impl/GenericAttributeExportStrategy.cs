using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public class GenericAttributeExportStrategy : CompiledExportStrategy, IGenericExportStrategy
	{
		private IEnumerable<Attribute> attributes;

		public GenericAttributeExportStrategy(Type exportType, IEnumerable<Attribute> attributes)
			: base(exportType)
		{
			this.attributes = attributes;
		}

		public override void Initialize()
		{
			foreach (Attribute attribute in attributes)
			{
				IExportAttribute exportAttribute = attribute as IExportAttribute;

				if (exportAttribute != null)
				{
					foreach (string provideExportName in exportAttribute.ProvideExportNames(exportType))
					{
						AddExportName(provideExportName);
					}

					foreach (Type provideExportType in exportAttribute.ProvideExportTypes(exportType))
					{
						AddExportType(provideExportType);
					}
				}
			}

			base.Initialize();
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			throw new Exception("You can't activate a generic type. The type must be closed into an compiled instance export.");
		}

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
						if (closingTypes[i].GetTypeInfo().ImplementedInterfaces.Any(x => x.GetTypeInfo().GUID == constraint.GetTypeInfo().GUID))
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

		public IExportStrategy CreateClosedStrategy(Type[] closingTypes)
		{
			try
			{
				Type closedType = exportType.MakeGenericType(closingTypes);

				return new ClosedAttributeExportStrategy(closedType, closedType.GetTypeInfo().GetCustomAttributes());
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
