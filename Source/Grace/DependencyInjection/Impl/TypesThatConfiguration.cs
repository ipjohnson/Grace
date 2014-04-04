using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This is the configuration object for TypesThat, developers are not intended to use this
	/// it is an internal class for Grace
	/// </summary>
	public class TypesThatConfiguration
	{
		private readonly List<Func<Type, bool>> filters = new List<Func<Type, bool>>(1);
		private bool useOr = false;

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <param name="attributeType"></param>
		/// <param name="attributeFilter"></param>
		/// <returns></returns>
		public TypesThatConfiguration HaveAttribute(Type attributeType, Func<Attribute, bool> attributeFilter = null)
		{
			Func<Type, bool> newFilter;

			if (attributeFilter != null)
			{
				newFilter = t => t.GetTypeInfo().GetCustomAttributes(attributeType, true).Any(attributeFilter);
			}
			else
			{
				newFilter = t => t.GetTypeInfo().GetCustomAttributes(attributeType, true).Any();
			}

			filters.Add(newFilter);

			return this;
		}

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="attributeFilter"></param>
		/// <returns></returns>
		public TypesThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
			where TAttribute : Attribute
		{
			Func<Type, bool> newFilter;

			if (attributeFilter != null)
			{
				newFilter = t => t.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).Any(
					x =>
					{
						bool returnValue = false;
						TAttribute attribute =
							x as TAttribute;

						if (attribute != null)
						{
							returnValue = attributeFilter(attribute);
						}

						return returnValue;
					});
			}
			else
			{
				newFilter = t => t.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).Any();
			}

			filters.Add(newFilter);

			return this;
		}

		/// <summary>
		/// Creates a new type filter method that returns true if the Name of the type starts with name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public TypesThatConfiguration StartWith(string name)
		{
			filters.Add(t => t.Name.StartsWith(name));

			return this;
		}

		/// <summary>
		/// Creates a new type filter that returns true if the Name ends with the provided string
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public TypesThatConfiguration EndWith(string name)
		{
			filters.Add(t => t.Name.EndsWith(name));

			return this;
		}

		/// <summary>
		/// Creates a new type filter based on the types namespace
		/// </summary>
		/// <param name="namespace"></param>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public TypesThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
		{
			Func<Type, bool> newFilter;

			if (includeSubnamespaces)
			{
				newFilter = type => type.Namespace == @namespace ||
										  type.Namespace != null &&
										  type.Namespace.StartsWith(@namespace + ".");
			}
			else
			{
				newFilter = type => type.Namespace == @namespace;
			}

			filters.Add(newFilter);

			return this;
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <param name="type"></param>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public TypesThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
		{
			return AreInTheSameNamespace(type.Namespace, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public TypesThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
		{
			return AreInTheSameNamespaceAs(typeof(T), includeSubnamespaces);
		}

		/// <summary>
		/// Or together the filters rather than using And
		/// </summary>
		public TypesThatConfiguration Or
		{
			get
			{
				useOr = true;

				return this;
			}
		}

		/// <summary>
		/// Automatically convert from TypefilterGroup to Func(Type,bool)
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static implicit operator Func<Type, bool>(TypesThatConfiguration configuration)
		{
			return new TypeFilterGroup(configuration.filters.ToArray()) { UseOr = configuration.useOr };
		}
	}
}