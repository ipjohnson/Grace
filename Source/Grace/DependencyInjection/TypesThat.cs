using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// That class provides Type filter methods to be used in ExportInterfaces and SelectTypes method
	/// </summary>
	public static class TypesThat
	{
		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <param name="attributeType"></param>
		/// <param name="attributeFilter"></param>
		/// <returns></returns>
		public static TypesThatConfiguration HaveAttribute(Type attributeType, Func<Attribute, bool> attributeFilter = null)
		{
			return new TypesThatConfiguration().HaveAttribute(attributeType, attributeFilter);
		}

		/// <summary>
		/// Tests to see if a type has an attribute
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="attributeFilter"></param>
		/// <returns></returns>
		public static TypesThatConfiguration HaveAttribute<TAttribute>(Func<TAttribute, bool> attributeFilter = null)
			where TAttribute : Attribute
		{
			return new TypesThatConfiguration().HaveAttribute(attributeFilter);
		}

		/// <summary>
		/// Creates a new type filter method that returns true if the Name of the type starts with name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static TypesThatConfiguration StartWith(string name)
		{
			return new TypesThatConfiguration().StartWith(name);
		}

		/// <summary>
		/// Creates a new type filter that returns true if the Name ends with the provided string
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static TypesThatConfiguration EndWith(string name)
		{
			return new TypesThatConfiguration().EndWith(name);
		}

		/// <summary>
		/// Creates a new type filter based on the types namespace
		/// </summary>
		/// <param name="namespace"></param>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public static TypesThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
		{
			return new TypesThatConfiguration().AreInTheSameNamespace(@namespace, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <param name="type"></param>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public static TypesThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
		{
			return new TypesThatConfiguration().AreInTheSameNamespaceAs(type, includeSubnamespaces);
		}

		/// <summary>
		/// Creates a new type filter that fiters based on if it's in the same namespace as another class
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeSubnamespaces"></param>
		/// <returns></returns>
		public static TypesThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
		{
			return new TypesThatConfiguration().AreInTheSameNamespaceAs<T>(includeSubnamespaces);
		}
	}
}