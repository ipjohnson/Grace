using Grace.DependencyInjection.Attributes;
using Xunit;

namespace Grace.UnitTests.Classes.Attributed
{
	public interface IComplexService
	{
		void Validate();
	}

	[Export(typeof(IComplexService))]
	public class ComplexService : IComplexService
	{
		private bool activationCalled;
		private bool importMethodCalled;

		public ComplexService(IAttributeBasicService basicService)
		{
			Assert.NotNull(basicService);
		}

		[Import]
		public IAttributeImportConstructorService ImportConstructorService { get; set; }

		public void Validate()
		{
			Assert.True(importMethodCalled);
			Assert.True(activationCalled);
		}

		[Import]
		public void ImportMethod(IAttributedImportPropertyService propertyService)
		{
			Assert.NotNull(propertyService);

			importMethodCalled = true;
		}

		[ActivationComplete]
		public void Activation()
		{
			activationCalled = true;
		}
	}
}