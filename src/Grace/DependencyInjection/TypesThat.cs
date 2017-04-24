using System;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Static class that offers type filters
    /// </summary>
    public static class TypesThat
    {
        /// <summary>
        /// Creates a type filter that returns true if a type has a particular property name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration HaveProperty(string propertyName)
        {
            return new TypesThatConfiguration().HaveProperty(propertyName);
        }

        /// <summary>
        /// Creates a type filter that returns true if a type has a particular property name
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration HaveProperty<T>(string propertyName = null)
        {
            return new TypesThatConfiguration().HaveProperty<T>(propertyName);
        }

        /// <summary>
        /// Creates a type filter that returns true if a type has a particular property name
        /// </summary>
        /// <param name="propertyType">property type</param>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration HaveProperty(Type propertyType, string propertyName = null)
        {
            return new TypesThatConfiguration().HaveProperty(propertyType, propertyName);
        }

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
        /// Provides a type filter for attributes, if true then the type will be used
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
        public static TypesThatConfiguration HaveAttribute(Func<Type, bool> consider)
        {
            return new TypesThatConfiguration().HaveAttribute(consider);
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
        /// Creates a new type filter that returns true if the Name contains the provided string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TypesThatConfiguration Contains(string name)
        {
            return new TypesThatConfiguration().Contains(name);
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

        /// <summary>
        /// Adds a type filter that returns true when a class is based on the specified type
        /// </summary>
        /// <typeparam name="T">based on type</typeparam>
        /// <returns>type filter</returns>
        public static TypesThatConfiguration AreBasedOn<T>()
        {
            return new TypesThatConfiguration().AreBasedOn<T>();
        }

        /// <summary>
        /// Adds a type filter that returns true when a class is based on the specified type
        /// </summary>
        /// <param name="type">based on type</param>
        /// <returns>type filter</returns>
        public static TypesThatConfiguration AreBasedOn(Type type)
        {
            return new TypesThatConfiguration().AreBasedOn(type);
        }

        /// <summary>
        /// Adds a type filter that filters a type based on it base type or interfaces
        /// </summary>
        /// <param name="typeFilter">type filter</param>
        /// <returns>type filter</returns>
        public static TypesThatConfiguration AreBasedOn(Func<Type, bool> typeFilter)
        {
            return new TypesThatConfiguration().AreBasedOn(typeFilter);
        }

        /// <summary>
        /// Adds a type filter
        /// </summary>
        /// <param name="matchFilter"></param>
        /// <returns>type filter</returns>
        public static TypesThatConfiguration Match(Func<Type, bool> matchFilter)
        {
            return new TypesThatConfiguration().Match(matchFilter);
        }

        /// <summary>
        /// Adds a type filter that returns true if the type is public
        /// </summary>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration ArePublic()
        {
            return new TypesThatConfiguration().ArePublic();
        }

        /// <summary>
        /// Adds a type filter that returns true if the type is private
        /// </summary>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration AreNotPublic()
        {
            return new TypesThatConfiguration().AreNotPublic();
        }

        /// <summary>
        /// Adds a type filter that returns true if the type is constructed generic
        /// </summary>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration AreConstructedGeneric()
        {
            return new TypesThatConfiguration().AreConstructedGeneric();
        }

        /// <summary>
        /// Adds a type filter that returns true if the type is an open generic
        /// </summary>
        /// <returns>configuration object</returns>
        public static TypesThatConfiguration AreOpenGeneric()
        {
            return new TypesThatConfiguration().AreOpenGeneric();
        }

        /// <summary>
        /// Not value
        /// </summary>
        /// <returns></returns>
        public static TypesThatConfiguration Not()
        {
            return new TypesThatConfiguration().Not;
        }
    }
}
