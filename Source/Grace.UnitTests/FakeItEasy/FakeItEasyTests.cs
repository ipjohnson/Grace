using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Grace.FakeItEasy;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.FakeItEasy
{
	public class FakeItEasyTests
	{
		[Fact]
		public void BasicFakeContainerTest()
		{
			FakeContainer container = new FakeContainer();

			ImportConstructorService importConstructorService =
				container.Locate<ImportConstructorService>();

			Assert.NotNull(importConstructorService);
			Assert.NotNull(importConstructorService.BasicService);
		}

		[Fact]
		public void ACallTo()
		{
			FakeContainer container = new FakeContainer();

			A.CallTo(() => container.Locate<IBasicService>(null, null).TestMethod()).Returns(5);

			ImportConstructorService importConstructorService =
				container.Locate<ImportConstructorService>();

			Assert.Equal(5, importConstructorService.TestMethod());

			A.CallTo(() => container.Locate<IBasicService>(null, null).TestMethod()).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Fact]
		public void ConfigureArrangeTest()
		{
			FakeContainer container = new FakeContainer();

			container.Configure(c => c.Fake<IBasicService>().
												Arrange(x => A.CallTo(() => x.TestMethod()).Returns(5)).
												AndSingleton());

			ImportConstructorService importConstructorService =
				container.Locate<ImportConstructorService>();

			Assert.Equal(5, importConstructorService.TestMethod());

			A.CallTo(() => container.Locate<IBasicService>(null, null).TestMethod()).MustHaveHappened(Repeated.Exactly.Once);
		}


		[Fact]
		public void ConfigureAssertTest()
		{
			FakeContainer container = new FakeContainer();

			container.Configure(c => c.Fake<IBasicService>().
												Arrange(x => A.CallTo(() => x.TestMethod()).Returns(5)).
												Assert(x => A.CallTo(() => x.TestMethod()).MustHaveHappened(Repeated.Exactly.Twice)));

			ImportConstructorService importConstructorService =
				container.Locate<ImportConstructorService>();

			Assert.Equal(5, importConstructorService.TestMethod());

			try
			{
				container.Assert();

				throw new Exception("This should have thrown an exception from the assert");
			}
			catch (ExpectationException)
			{
				// we are expecting this
			}
		}
	}
}
