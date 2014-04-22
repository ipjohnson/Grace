using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IPropertedPropertyImportClass
	{
		IBasicService BasicService { get; }

		bool ProtectedIsNull { get; }
	}

	public class PropertedPropertyImportClass : IPropertedPropertyImportClass
	{
		public IBasicService BasicService { get; set; }

		public bool ProtectedIsNull
		{
			get { return SomeOtherBasicService == null; }
		}

		protected IBasicService SomeOtherBasicService { get; set; }


	}
}
