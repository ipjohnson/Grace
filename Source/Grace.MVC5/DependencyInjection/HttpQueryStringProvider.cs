using System;
using System.Web;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{
	/// <summary>
	/// IExportValueProvider that fetches it's value from Http query  
	/// </summary>
	public class HttpQueryStringProvider : IExportValueProvider
	{
		public HttpQueryStringProvider(string parameterName = null)
		{
			ParameterName = parameterName;
		}

		private string ParameterName { get; set; }

		/// <summary>
		/// Activate value
		/// </summary>
		/// <param name="exportInjectionScope">injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">consider filter</param>
		/// <returns>activated value</returns>
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			string parameterName = ParameterName;

			if (context.TargetInfo == null)
			{
				throw new Exception("Must be injecting into type");
			}

			if (string.IsNullOrEmpty(parameterName))
			{
				parameterName = context.TargetInfo.InjectionTargetName;
			}

			string propertyString = HttpContext.Current.Request.QueryString[parameterName];

			if (context.TargetInfo.InjectionTargetType != typeof(string) && !string.IsNullOrEmpty(propertyString))
			{
				Type underlyingType = Nullable.GetUnderlyingType(context.TargetInfo.InjectionTargetType);

				if (underlyingType != null)
				{
					return Convert.ChangeType(propertyString, underlyingType);
				}

				if (context.TargetInfo.InjectionTargetType.IsValueType)
				{
					return Convert.ChangeType(propertyString, context.TargetInfo.InjectionTargetType);
				}

				throw new ArgumentException("You can only import ValueTypes or Nullable Types");
			}

			return propertyString;
		}
	}
}