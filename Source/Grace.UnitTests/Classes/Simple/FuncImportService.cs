using System;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IFuncImportService
	{
		IBasicService GetBasicService();
	}

	public class FuncImportService : IFuncImportService
	{
		private readonly Func<IBasicService> basicServiceFunc;

		public FuncImportService(Func<IBasicService> basicServiceFunc)
		{
			this.basicServiceFunc = basicServiceFunc;
		}

		public IBasicService GetBasicService()
		{
			return basicServiceFunc();
		}
	}
}