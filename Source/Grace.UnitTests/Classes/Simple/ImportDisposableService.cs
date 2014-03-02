using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportDisposableService
	{
		
	}

	public class ImportDisposableService : IImportDisposableService
	{
		public ImportDisposableService(IDisposableService disposableService)
		{
			
		}
	}
}
