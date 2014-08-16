using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// This attribute is used to mark a type for export. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true,
		Inherited = true)]
	public class ExportAttribute : Attribute, IExportAttribute
	{
		/// <summary>
		/// Default constructor. Export as the class name
		/// </summary>
		public ExportAttribute()
		{
		}

		/// <summary>
		/// exports using the provided export name
		/// </summary>
		/// <param name="exportName"></param>
		/// <param name="extraNames"></param>
		public ExportAttribute(string exportName, params string[] extraNames)
		{
			if (exportName == null)
			{
				throw new ArgumentNullException("exportName", "You must provide an export name");
			}

			List<string> exportList = new List<string> { exportName };

			if (extraNames != null)
			{
				exportList.AddRange(extraNames);
			}

			ExportNames = exportList;
		}

		/// <summary>
		/// Export by type rather than by name
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="extraTypes"></param>
		public ExportAttribute(Type exportType, params Type[] extraTypes)
		{
			if (exportType == null)
			{
				throw new ArgumentNullException("exportType", "You must provide a type");
			}

			List<Type> exportList = new List<Type> { exportType };

			if (extraTypes != null)
			{
				foreach (Type extraType in extraTypes)
				{
					exportList.Add(extraType);
				}
			}

			ExportTypes = exportList;
		}

		/// <summary>
		/// The list of export names
		/// </summary>
		protected List<string> ExportNames { get; set; }

		/// <summary>
		/// List of export types
		/// </summary>
		protected List<Type> ExportTypes { get; set; }

		/// <summary>
		/// Provide a list of export names that the type should be exported as
		/// </summary>
		/// <param name="attributedType">type that was attributed</param>
		/// <returns>list of export names</returns>
		public IEnumerable<string> ProvideExportNames(Type attributedType)
		{
			if (ExportNames != null)
			{
				return ExportNames;
			}

			return ImmutableArray<string>.Empty;
		}

		/// <summary>
		/// Provide a list of types to export as
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IEnumerable<Type> ProvideExportTypes(Type attributedType)
		{
			if (ExportTypes != null)
			{
				return ExportTypes;
			}

			return ImmutableArray<Type>.Empty;
		}
	}
}