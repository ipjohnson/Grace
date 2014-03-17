using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class OpenGenericUtilitiesTests
	{
		[Fact]
		public void SimpleCloseInterfaceTest()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(typeof(GenericService<>), typeof(IGenericService<bool>));

			Assert.Equal(typeof(GenericService<bool>), closedType);
		}

		[Fact]
		public void PartialCloseTest()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(typeof(PartialOpenGenericString<>), typeof(IPartialInterface<int, string>));

			Assert.Equal(typeof(PartialOpenGenericString<int>), closedType);
		}

		[Fact]
		public void Constraint()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(typeof(ConstrainedService<>), typeof(IGenericService<IBasicService>));

			Assert.Equal(typeof(ConstrainedService<IBasicService>), closedType);
		}

		[Fact]
		public void FailedConstraint()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(typeof(ConstrainedService<>), typeof(IGenericService<bool>));

			Assert.Null(closedType);
		}

		[Fact]
		public void OpenGenericAttributeCosntraints()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(
					typeof(MultipleOpenGenericConstrained<,,,>),
					typeof(IMultipleOpenGeneric<BasicService, IBasicService, bool, DateTime>));

			Assert.Equal(typeof(MultipleOpenGenericConstrained<BasicService, IBasicService, bool, DateTime>), closedType);
		}

		[Fact]
		public void MissingNewConstraint()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(
					typeof(MultipleOpenGenericConstrained<,,,>),
					typeof(IMultipleOpenGeneric<ImportConstructorService, IBasicService, bool, DateTime>));

			Assert.Null(closedType);
		}

		[Fact]
		public void MissingStructConstraint()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(
					typeof(MultipleOpenGenericConstrained<,,,>),
					typeof(IMultipleOpenGeneric<BasicService, IBasicService, bool, BasicService>));

			Assert.Null(closedType);
		}

		[Fact]
		public void MissingClassConstraint()
		{
			Type closedType =
				OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(
					typeof(MultipleOpenGenericConstrained<,,,>),
					typeof(IMultipleOpenGeneric<BasicService, int, bool, DateTime>));

			Assert.Null(closedType);
		}
	}
}
