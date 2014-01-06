using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	public interface IImportFilterAttribute
	{
		ExportStrategyFilter ProvideFilter(Type attributedType, string attributedName);
	}
}
