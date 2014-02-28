using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class ExceptionTests
	{
		[Fact]
		public void ThrowsMissingException()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			try
			{
				IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

				throw new Exception("Should not reach this point");
			}
			catch (MissingDependencyException exp)
			{
				// we should be throwing this exception
			}
		}

		[Fact]
		public void GeneralLocateException()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.ExportInstance<IBasicService>((s, context) => { throw new Exception("Some exception"); });
			                    });

			try
			{
				IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

				throw new Exception("Should not reach this point");
			}
			catch (GeneralLocateException exp)
			{
				// we should be throwing this exception
			}
		}
	}
}
