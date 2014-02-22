using System;
using FakeItEasy;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.FakeItEasy
{
	public class FakeExportStrategy<T> : ConfigurableExportStrategy
	{
		private Action<T> arrangeAction;
		private Action<T> assertAction;

		public FakeExportStrategy()
			: base(typeof(T))
		{
		}

		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			if (lifestyle != null)
			{
				return lifestyle.Locate(InternalActivate, exportInjectionScope, context, this);
			}

			return InternalActivate(exportInjectionScope, context);
		}

		private object InternalActivate(IInjectionScope injectionscope, IInjectionContext context)
		{
			T fakeT = A.Fake<T>();

			if (arrangeAction != null)
			{
				arrangeAction(fakeT);
			}

			IFakeCollection fakeCollection = injectionscope.Locate<IFakeCollection>();

			if (fakeCollection != null)
			{
				fakeCollection.AddFake(fakeT, assertAction);
			}

			return fakeT;
		}

		public void Arrange(Action<T> arrangeAction)
		{
			this.arrangeAction += arrangeAction;
		}

		public void Assert(Action<T> assertAction)
		{
			this.assertAction += assertAction;
		}
	}
}