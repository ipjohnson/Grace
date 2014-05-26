using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Utilities
{
	/// <summary>
	/// MethodInfo wrapper class
	/// </summary>
	public class MethodInfoWrapper
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="methodInfo">method info to wrap</param>
		public MethodInfoWrapper(MethodInfo methodInfo)
		{
			MethodInfo = methodInfo;
		}

		/// <summary>
		/// Wrapped method info
		/// </summary>
		public MethodInfo MethodInfo { get; private set; }

		/// <summary>
		/// Convert to method info
		/// </summary>
		/// <param name="wrapper">wrapper</param>
		/// <returns>method info</returns>
		public static implicit operator MethodInfo(MethodInfoWrapper wrapper)
		{
			return wrapper.MethodInfo;
		}

		/// <summary>
		/// Convert to wrapped method info
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <returns></returns>
		public static implicit operator MethodInfoWrapper(MethodInfo methodInfo)
		{
			return new MethodInfoWrapper(methodInfo);
		}
	}
}
