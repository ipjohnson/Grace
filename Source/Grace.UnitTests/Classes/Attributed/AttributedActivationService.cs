using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Xunit;

namespace Grace.UnitTests.Classes.Attributed
{
	public interface IAttributedActivationService
	{
		bool SimpleActivationCalled { get; }

		bool ContextActivationCalled { get; }
	}

	[Export(typeof(IAttributedActivationService))]
	public class AttributedActivationService : IAttributedActivationService
	{
		public bool SimpleActivationCalled { get; private set; }

		public bool ContextActivationCalled { get; private set; }

		[ActivationComplete]
		public void SimpleActivate()
		{
			SimpleActivationCalled = true;
		}

		[ActivationComplete]
		public void InjectionContextActivation(IInjectionContext context)
		{
			Assert.NotNull(context);

			ContextActivationCalled = true;
		}
	}
}