using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.WCF.DependencyInjection
{
	public class WCFSharedPerRequestLifestyleProvider : IPerRequestLifestyleProvider
	{
		public ILifestyle ProvideContainer()
		{
			return new WCFSharedPerRequestLifestyle();
		}
	}
}
