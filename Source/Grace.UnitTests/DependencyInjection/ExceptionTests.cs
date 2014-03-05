using System;
using System.Collections.Generic;
using System.IO;
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
				Console.WriteLine(exp.Message);
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

				Console.WriteLine(exp.Message);
				// we should be throwing this exception
			}
		}

		[Fact(Timeout = 0)]
		public void CircularDependencyTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<DependencyA>().ByInterfaces();
										  c.Export<DependencyB>().ByInterfaces();
									  });

			try
			{
				var circle = container.Locate<IDependencyA>();

				throw new Exception("Should not have gotten here");
			}
			catch (CircularDependencyDetectedException exp)
			{
				Console.WriteLine(exp.Message);
				// this is what we want
			}

		}

		[Fact]
		public void DisposalScopeMissingException()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportDisposableService>().ByInterfaces().AndWeakSingleton();
				                    c.Export<DisposableService>().ByInterfaces();
			                    });

			try
			{
				var service = container.Locate<IImportDisposableService>();

				throw new Exception("Should not get here");
			}
			catch (DisposalScopeMissingException exp)
			{
				Console.WriteLine(exp.Message);
				// we want this
			}

		}
	}
}
