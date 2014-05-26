using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Utilities
{
	/// <summary>
	/// Wrapper class around PropertyInfo
	/// </summary>
	public class PropertyInfoWrapper
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="propertyInfo">property info to wrap</param>
		public PropertyInfoWrapper(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;
		}

		/// <summary>
		/// Wrapped property info
		/// </summary>
		public PropertyInfo PropertyInfo { get; private set; }

		/// <summary>
		/// Convert to propery info
		/// </summary>
		/// <param name="wrapper">wrapped info</param>
		/// <returns>property info</returns>
		public static implicit operator PropertyInfo(PropertyInfoWrapper wrapper)
		{
			return wrapper.PropertyInfo;
		}

		/// <summary>
		/// convert to wrapped property info
		/// </summary>
		/// <param name="propertyInfo">property info</param>
		/// <returns>wrapped property info</returns>
		public static implicit operator PropertyInfoWrapper(PropertyInfo propertyInfo)
		{
			return new PropertyInfoWrapper(propertyInfo);
		}
	}
}
