using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Grace.DependencyInjection.Configuration
{
	public class BaseElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement
	{
		private readonly string elementName;

		public BaseElementCollection(string elementName)
		{
			this.elementName = elementName;
		}

		protected override string ElementName
		{
			get { return elementName; }
		}

		protected override bool IsElementName(string elementName)
		{
			return this.elementName == elementName;
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return (ConfigurationElement)Activator.CreateInstance(typeof(T));
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return Guid.NewGuid();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T tObject in (IEnumerable)this)
			{
				yield return tObject;
			}
		}

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