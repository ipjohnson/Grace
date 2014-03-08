using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Base collection of elements
	/// </summary>
	/// <typeparam name="T">element type</typeparam>
	public class BaseElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement, new()
	{
		private readonly string elementName;

		/// <summary>
		/// Default constructor takes name of element
		/// </summary>
		/// <param name="elementName"></param>
		public BaseElementCollection(string elementName)
		{
			this.elementName = elementName;
		}

		/// <summary>
		/// Element name
		/// </summary>
		protected override string ElementName
		{
			get { return elementName; }
		}

		/// <summary>
		/// Is element name equal
		/// </summary>
		/// <param name="elementName">element name</param>
		/// <returns>true if names are equal</returns>
		protected override bool IsElementName(string elementName)
		{
			return this.elementName == elementName;
		}

		/// <summary>
		/// Create a new element
		/// </summary>
		/// <returns>new element</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}

		/// <summary>
		/// Returns collection type
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		/// <summary>
		/// Get a key for the element
		/// </summary>
		/// <param name="element">element</param>
		/// <returns>new key</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return Guid.NewGuid();
		}

		/// <summary>
		/// Gets an enumerator for the collection
		/// </summary>
		/// <returns>enumerator</returns>
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
		/// <typeparam name="TValue">property type</typeparam>
		/// <param name="propertyName">property name</param>
		/// <returns>property vaue</returns>
		protected TValue PropertyValue<TValue>([CallerMemberName] String propertyName = null)
		{
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}

			string attributeName = Char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

			return (TValue)this[attributeName];
		}
	}
}