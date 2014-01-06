using System;

namespace Grace.UnitTests.Classes.Simple
{
	public class FuncTypeImportService
	{
		private readonly Func<Type, object> funcObject;

		public FuncTypeImportService(Func<Type, object> funcObject)
		{
			this.funcObject = funcObject;
		}

		public object Create(Type objectType)
		{
			return funcObject(objectType);
		}
	}
}