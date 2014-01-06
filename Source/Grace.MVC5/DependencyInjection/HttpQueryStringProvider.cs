using System;
using System.Web;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{
	public class HttpQueryStringProvider : IExportValueProvider
	{
		public HttpQueryStringProvider(string parameterName = null)
		{
			ParameterName = parameterName;
		}

		private string ParameterName { get; set; }

		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			string parameterName = ParameterName;

			if (string.IsNullOrEmpty(parameterName))
			{
				if (context.TargetInfo == null)
				{
					throw new Exception("Must be injecting into type");
				}

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