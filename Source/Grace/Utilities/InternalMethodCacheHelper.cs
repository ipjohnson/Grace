using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Utilities
{
	internal static class InternalMethodCacheHelper
	{
		internal static SafeDictionary<MethodInfo, object> WeakDelegates = new SafeDictionary<MethodInfo, object>();
	}
}
