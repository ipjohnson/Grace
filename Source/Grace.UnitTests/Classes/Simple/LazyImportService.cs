using System;

namespace Grace.UnitTests.Classes.Simple
{
	public class LazyImportService
	{
		private readonly Lazy<IBasicService> lazyBasic;

		public LazyImportService(Lazy<IBasicService> lazyBasic)
		{
			this.lazyBasic = lazyBasic;
		}

		public IBasicService BasicService
		{
			get { return lazyBasic.Value; }
		}
	}
}