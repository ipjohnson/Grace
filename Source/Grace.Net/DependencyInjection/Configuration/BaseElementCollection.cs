using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class BaseElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement
	{
		private string elementName;

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
	}
}
