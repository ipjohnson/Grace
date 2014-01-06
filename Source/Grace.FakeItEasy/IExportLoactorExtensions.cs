using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.FakeItEasy
{
	/// <summary>
	/// Export locator extensions
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportLoactorExtensions
	{
		/// <summary>
		/// Sets up a Container or IInjectionScope to Fake exports. If you are using a FakeContainer you do not need to call this
		/// </summary>
		/// <param name="locator"></param>
		public static void FakeIt(this IExportLocator locator)
		{
			locator.AddSecondaryLocator(new FakeExportLocator());
		}

		/// <summary>
		/// Assert all fakes pass their arrange statements
		/// </summary>
		/// <param name="locator">container or injection scope that was faked</param>
		public static void Assert(this IExportLocator locator)
		{
			IFakeCollection fakeCollection = locator.Locate<IFakeCollection>();

			if (fakeCollection == null)
			{
				throw new Exception("This container or scope has not had the FakeIt method run");
			}

			fakeCollection.Assert();
		}
	}
}
