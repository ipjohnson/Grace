using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	public interface IGenericExportStrategy : IExportStrategy
	{
		bool CheckGenericConstrataints(Type[] closingTypes);

		IExportStrategy CreateClosedStrategy(Type[] closingTypes);
	}
}
