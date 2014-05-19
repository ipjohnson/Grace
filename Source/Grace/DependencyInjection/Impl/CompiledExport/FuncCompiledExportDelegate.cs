using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.CompiledExport
{
	/// <summary>
	/// This class creates a compiled delegate that can be used for export
	/// It uses a function to create a new instance
	/// </summary>
	public class FuncCompiledExportDelegate : BaseCompiledExportDelegate
	{
		private static readonly MethodInfo invokeActivationDelegateInfo;
		private readonly ExportActivationDelegate exportActivationDelegate;

		static FuncCompiledExportDelegate()
		{
			invokeActivationDelegateInfo =
				typeof(FuncCompiledExportDelegate).GetRuntimeMethod("InvokeActivationDelegate",
					new[]
					{
						typeof(ExportActivationDelegate),
						typeof(IInjectionScope),
						typeof(IInjectionContext)
					});
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportDelegateInfo"></param>
		/// <param name="activationDelegate"></param>
		/// <param name="exportStrategy"></param>
		/// <param name="owningScope"></param>
		public FuncCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo,
			ExportActivationDelegate activationDelegate,
			IExportStrategy exportStrategy,
			IInjectionScope owningScope)
			: base(exportDelegateInfo,exportStrategy, owningScope)
		{
			exportActivationDelegate = activationDelegate;
		}

		/// <summary>
		/// This method creates the expression that calls the constructor or function to create a new isntance that will be returned
		/// </summary>
		protected override void CreateInstantiationExpression()
		{
			Expression callExpression =
				Expression.Call(invokeActivationDelegateInfo,
					Expression.Constant(exportActivationDelegate),
					exportStrategyScopeParameter,
					injectionContextParameter);

			Expression castExpression = Expression.Convert(callExpression, exportDelegateInfo.ActivationType);

			instanceExpressions.Add(Expression.Assign(instanceVariable, castExpression));
		}

		/// <summary>
		/// Method used to invoke an activation delegate
		/// </summary>
		/// <param name="exportStrategyScope"></param>
		/// <param name="activationDelegate"></param>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		public static object InvokeActivationDelegate(ExportActivationDelegate activationDelegate,
			IInjectionScope exportStrategyScope,
			IInjectionContext injectionContext)
		{
			return activationDelegate(exportStrategyScope, injectionContext);
		}
	}
}