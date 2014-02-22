using System;

namespace Grace.DependencyInjection.Impl
{
	public interface IGenericExportStrategy : IExportStrategy
	{
		bool CheckGenericConstrataints(Type[] closingTypes);

		IExportStrategy CreateClosedStrategy(Type[] closingTypes);
	}
}