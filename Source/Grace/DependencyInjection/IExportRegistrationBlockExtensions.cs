using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Extension methods for registration block
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Ups the priority of partially closed generics based on the number of closed parameters
		/// </summary>
		/// <param name="registrationBlock">registration block</param>
		public static void PrioritizePartiallyClosedGenerics(this IExportRegistrationBlock registrationBlock)
		{
			registrationBlock.AddInspector(new PartiallyClosedGenericPriorityAugmenter());
		}

		/// <summary>
		/// Register a configuration module
		/// </summary>
		/// <typeparam name="T">module type</typeparam>
		public static void RegisterModule<T>(this IExportRegistrationBlock registrationBlock) where T : IConfigurationModule, new()
		{
			registrationBlock.RegisterModule(new T());
		}

		/// <summary>
		/// Register a configuration module
		/// </summary>
		/// <typeparam name="T">module type</typeparam>
		/// <param name="registrationBlock">registration block</param>
		/// <param name="module">configuration module</param>
		public static void RegisterModule<T>(this IExportRegistrationBlock registrationBlock, T module) where T : IConfigurationModule
		{
			if (ReferenceEquals(null, module))
			{
				throw new ArgumentNullException("module");
			}

			module.Configure(registrationBlock);
		}

		/// <summary>
		/// This is a short cut to registering a value as a name using the member name for exporting
		/// ExportNamedValue(() => someValue) export the value of someValue under the name someValue
		/// ExportInstance(someValue).AsName("someValue") is the long hand form
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registrationBlock"></param>
		/// <param name="valueExpression"></param>
		/// <returns></returns>
		public static IFluentExportInstanceConfiguration<T> ExportNamedValue<T>(
			this IExportRegistrationBlock registrationBlock,
			Expression<Func<T>> valueExpression)
		{
			MemberExpression memberExpression = valueExpression.Body as MemberExpression;
			string exportName = null;

			if (memberExpression != null)
			{
				exportName = memberExpression.Member.Name;
			}

			if (exportName != null)
			{
				Func<T> func = valueExpression.Compile();

				return registrationBlock.ExportInstance((s, c) => func()).AsName(exportName);
			}

			throw new Exception("This method can only be used on members (i.e. ExportNamedValue(() => SomeProperty))");
		}
	}
}
