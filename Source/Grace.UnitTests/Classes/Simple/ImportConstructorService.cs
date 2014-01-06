using System;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportConstructorService
	{
		IBasicService BasicService { get; }

		int TestMethod();
	}

	[SomeTest]
	public class ImportConstructorService : IImportConstructorService
	{
		public ImportConstructorService([SomeTest] IBasicService basicService)
		{
			if (basicService == null)
			{
				throw new ArgumentNullException("basicService");
			}

			BasicService = basicService;
		}

		public IBasicService BasicService { get; private set; }

		public int TestMethod()
		{
			return BasicService.TestMethod();
		}
	}
}