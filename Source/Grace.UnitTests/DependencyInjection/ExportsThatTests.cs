using System;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class ExportsThatTests
	{
		[Fact]
		public void HaveAttribute()
		{
			ExportStrategyFilter filter = ExportsThat.HaveAttribute(typeof(SomeTestAttribute));

			FauxExportStrategy failStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[0]
			                                  };

			FauxExportStrategy passStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[] { new SomeTestAttribute() }
			                                  };

			Assert.True(filter(new FauxInjectionContext(), passStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy));
		}

		[Fact]
		public void HaveAttributeFiltered()
		{
			ExportStrategyFilter filter = ExportsThat.HaveAttribute(typeof(SomeTestAttribute),
				attribute => ((SomeTestAttribute)attribute).TestValue == 5);

			FauxExportStrategy failStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[0]
			                                  };

			FauxExportStrategy failStrategy2 = new FauxExportStrategy(() => new object())
			                                   {
				                                   Attributes = new Attribute[] { new SomeTestAttribute() }
			                                   };

			FauxExportStrategy passStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[] { new SomeTestAttribute { TestValue = 5 } }
			                                  };

			Assert.True(filter(new FauxInjectionContext(), passStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy2));
		}

		[Fact]
		public void HaveAttributeGeneric()
		{
			ExportStrategyFilter filter = ExportsThat.HaveAttribute<SomeTestAttribute>();

			FauxExportStrategy failStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[0]
			                                  };

			FauxExportStrategy passStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[] { new SomeTestAttribute() }
			                                  };

			Assert.True(filter(new FauxInjectionContext(), passStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy));
		}

		[Fact]
		public void HaveAttributeGenericFiltered()
		{
			ExportStrategyFilter filter = ExportsThat.HaveAttribute<SomeTestAttribute>(attribute => attribute.TestValue == 5);

			FauxExportStrategy failStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[0]
			                                  };

			FauxExportStrategy failStrategy2 = new FauxExportStrategy(() => new object())
			                                   {
				                                   Attributes = new Attribute[] { new SomeTestAttribute() }
			                                   };

			FauxExportStrategy passStrategy = new FauxExportStrategy(() => new object())
			                                  {
				                                  Attributes = new Attribute[] { new SomeTestAttribute { TestValue = 5 } }
			                                  };

			Assert.True(filter(new FauxInjectionContext(), passStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy));
			Assert.False(filter(new FauxInjectionContext(), failStrategy2));
		}
	}
}