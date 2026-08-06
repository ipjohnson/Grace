using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	using Adapter = Func<object, Type, string, ImportAttributeInfo>;
	/// <summary>
	/// Information about how the import should be performed
	/// </summary>
	public class ImportAttributeInfo
	{
		private static readonly Dictionary<Type, Adapter> adapters = new Dictionary<Type, Adapter>(16);

		/// <summary>
		/// Register an adapter for an attribute that cannot implement IImportAttribute
		/// </summary>
		public static void RegisterImportAttributeAdapter<T>(Adapter adapter) where T : Attribute
			=> adapters.Add(typeof(T), adapter);

		/// <summary>
		/// Factory method to get ImportAttributeInfo for a specific reflection member
		/// </summary>
		public static ImportAttributeInfo For(
			ICustomAttributeProvider attributeProvider,
			Type activationType,
			string name)
		{
			foreach (var attr in attributeProvider.GetCustomAttributes(true))
			{
				ImportAttributeInfo info = null;
				
				if (attr is IImportAttribute iia) 
				{
					info = iia.ProvideImportInfo(activationType, name);
				}
				else if (adapters.TryGetValue(attr.GetType(), out var adapter))
				{
					info = adapter(attr, activationType, name);
				}

				if (info != null)
				{
					return info;
				}
			}

			return null;
		}

		/// <summary>
		/// Default value
		/// </summary>
		public object DefaultValue { get; set; }

		/// <summary>
		/// Is the import required
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// The key that should be used when importing
		/// </summary>
		public object ImportKey { get; set; }
        
		/// <summary>
		/// Import Filter 
		/// </summary>
		public ActivationStrategyFilter ExportStrategyFilter { get; set; }

		/// <summary>
		/// Comparer object for import
		/// </summary>
		public object Comparer { get; set; }		
	}

	/// <summary>
	/// Attributes that implement this interface will be included while discovering attributes for importing
	/// </summary>
	public interface IImportAttribute
	{
		/// <summary>
		/// Provides information about the import
		/// </summary>
		/// <param name="attributedType">the type that is attributed, null when attributed on methods</param>
		/// <param name="attributedName">the name of the method, property, or parameter name</param>
		/// <returns></returns>
		ImportAttributeInfo ProvideImportInfo(Type attributedType, string attributedName);
	}
}