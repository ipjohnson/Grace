using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data
{
	public delegate object GetPropertyDelegate(object instanceobject, object index, bool throwIfPathMissing);

	public delegate void SetPropertyDelegate(
		object instance, object newPropertyValue, object index, bool createIfPathMissing);

	public delegate object CallMethodDelegate(object instance, bool createIfPathMissing, params object[] parameters);

	/// <summary>
	/// Simple service that allows the caller to access properties and call method on objects 
	/// the only have string names for.
	/// The Default implementation uses cached compiled Linq Expressions for performance reasons
	/// </summary>
	public interface IReflectionService
	{
		/// <summary>
		/// Gets a named property value from an object
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name (can be nested A.B.C.D)</param>
		/// <returns>property value</returns>
		object GetPropertyValue(object valueObject, string propertyName);

		/// <summary>
		/// Gets a named property value from an object
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name (can be nested A.B.C.D)</param>
		/// <param name="index">index for final property</param>
		/// <param name="throwIfPathMissing">throw an exception if any part of the path is missing</param>
		/// <returns>property value</returns>
		object GetPropertyValue(object valueObject,
										string propertyName,
										object index,
										bool throwIfPathMissing);

		/// <summary>
		/// Sets a value into a named Property
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name to fetch</param>
		/// <param name="newValue"></param>
		bool SetPropertyValue(object valueObject, string propertyName, object newValue);

		/// <summary>
		/// Sets a value into a named Property
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name to fetch</param>
		/// <param name="newValue"></param>
		/// <param name="index"></param>
		/// <param name="createIfPathMissing"></param>
		bool SetPropertyValue(object valueObject,
									 string propertyName,
									 object newValue,
									 object index,
									 bool createIfPathMissing);

		/// <summary>
		/// Calls a method in the target by a specified name.
		/// For a method with a return type of void use T type of object
		/// </summary>
		/// <typeparam name="T">the return type of the method</typeparam>
		/// <param name="target">the target object</param>
		/// <param name="methodName">method name (can be dotted form A.B.C</param>
		/// <param name="throwIfPathMissing">throw an exception if part of the path is missing</param>
		/// <param name="parameters">parameters to the method</param>
		/// <returns>the value the method returns</returns>
		object CallMethod(object target, string methodName, bool throwIfPathMissing, params object[] parameters);

		/// <summary>
		/// Creates a new delegate that can be used to access a property in an object by property name
		/// </summary>
		/// <param name="instanceType">object type to target</param>
		/// <param name="propertyName">property name (can be dotted form A.B.C)</param>
		/// <returns>new property delegate</returns>
		GetPropertyDelegate CreateGetPropertyDelegate(Type instanceType, string propertyName, Type indexType);

		/// <summary>
		/// Creates a new delegate that be used to set a property on an object by property name
		/// </summary>
		/// <param name="instanceType">object type to target</param>
		/// <param name="propertyName">property name (can be dotted form A.B.C)</param>
		/// <returns>new property delegate</returns>
		SetPropertyDelegate CreateSetPropertyDelegate(Type instanceType, string propertyName, Type indexType);
	}
}
