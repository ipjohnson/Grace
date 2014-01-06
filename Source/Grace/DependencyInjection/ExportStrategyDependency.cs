using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Represents the type of dependency for the export
	/// </summary>
	public enum ExportStrategyDependencyType
	{
		/// <summary>
		/// A constructor parameter
		/// </summary>
		ConstructorParameter,

		/// <summary>
		/// A Property
		/// </summary>
		Property,

		/// <summary>
		/// A method parameter
		/// </summary>
		MethodParameter
	}

	/// <summary>
	/// Export strategy dependency
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplayString,nq}")]
	public class ExportStrategyDependency
	{
		/// <summary>
		/// Type of dependency
		/// </summary>
		public ExportStrategyDependencyType DependencyType { get; set; }

		/// <summary>
		/// Name of the target being injected (ParameterName or PropertyName)
		/// </summary>
		public string TargetName { get; set; }

		/// <summary>
		/// Type being imported, can be null when locating by name
		/// </summary>
		public Type ImportType { get; set; }

		/// <summary>
		/// name of export, can be null when locating by type
		/// </summary>
		public string ImportName { get; set; }

		/// <summary>
		/// Has Value Provider
		/// </summary>
		public bool HasValueProvider { get; set; }

		/// <summary>
		/// Has a filter for it's dependency
		/// </summary>
		public bool HasFilter { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal string DebuggerDisplayString
		{
			get
			{
				if (ImportType != null)
				{
					return ImportType.FullName;
				}

				return ImportName;
			}
		}
	}
}
