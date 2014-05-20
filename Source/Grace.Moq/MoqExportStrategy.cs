using System;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// Export strategy the creates a Moq object
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MoqExportStrategy<T> : ConfigurableExportStrategy where T : class
	{
		private bool initialized;
		private Action<Mock<T>> arrangeMethod;
		private Action<Mock<T>> assertMethod;

		/// <summary>
		/// Default constructor
		/// </summary>
		public MoqExportStrategy()
			: base(typeof(Mock<T>))
		{
		}

		/// <summary>
		/// Activate the Mock object
		/// </summary>
		/// <param name="exportInjectionScope">export scope for this activation</param>
		/// <param name="context">context for the activation</param>
		/// <param name="consider">filter to consider when activating</param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			if (lifestyle != null)
			{
				return lifestyle.Locate(InternalActivate, exportInjectionScope, context, this);
			}

			return InternalActivate(exportInjectionScope, context);
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public override void Initialize()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			AddExportType(typeof(Mock<T>));

			PropertyInfo propertyInfo = typeof(Mock).GetProperty("Object");

			PropertyExportStrategy objectExport = new PropertyExportStrategy(propertyInfo, this, null);

			objectExport.AddExportType(typeof(T));

			AddSecondaryExport(objectExport);

			if (arrangeMethod != null)
			{
				EnrichWithDelegate((scope, context, injectObject) =>
				                   {
					                   arrangeMethod((Mock<T>)injectObject);

					                   return injectObject;
				                   });
			}

			base.Initialize();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="arrangeMethod"></param>
		public void Arrange(Action<Mock<T>> arrangeMethod)
		{
			this.arrangeMethod += arrangeMethod;
		}

		/// <summary>
		/// Add a verify method to the export
		/// </summary>
		/// <param name="assertMethod"></param>
		public void Assert(Action<Mock<T>> assertMethod)
		{
			this.assertMethod += assertMethod;
		}

		private object InternalActivate(IInjectionScope injectionScope, IInjectionContext context)
		{
			Mock<T> mock = new Mock<T>();
			object returnValue = mock;

			if (enrichWithDelegates != null)
			{
				foreach (EnrichWithDelegate enrichWithDelegate in enrichWithDelegates)
				{
					returnValue = enrichWithDelegate(injectionScope, context, mock);
				}
			}

			IMockCollection mockCollection = injectionScope.Locate<IMockCollection>();

			if (mockCollection != null)
			{
				mockCollection.AddMock(mock, assertMethod);
			}

			return returnValue;
		}
	}
}