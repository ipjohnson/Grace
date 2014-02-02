using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class BaseElement : ConfigurationElement
	{
		/// <summary>
		/// get the property value, note the first character will be made lower case
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected T PropertyValue<T>([CallerMemberName] String propertyName = null)
		{
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}

			string attributeName = Char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

			return (T)this[attributeName];
		}
	}
}
