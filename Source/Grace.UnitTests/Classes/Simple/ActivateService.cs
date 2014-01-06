using System;
using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IActiveService
	{
		bool SimpleActivateCalled { get; }

		bool InjectionContextActivateCalled { get; }
	}

	public class ActivateService : IActiveService
	{
		public bool SimpleActivateCalled { get; private set; }

		public bool InjectionContextActivateCalled { get; private set; }

		public void SimpleActivate()
		{
			SimpleActivateCalled = true;
		}

		public void InjectionContextActivate(IInjectionContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			InjectionContextActivateCalled = true;
		}
	}
}