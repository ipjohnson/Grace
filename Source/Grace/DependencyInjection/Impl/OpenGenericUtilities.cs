using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Class contains helper method for working with open generic methods
	/// </summary>
	public static class OpenGenericUtilities
	{
		/// <summary>
		/// Creates a closed type using the requested type parameters.
		/// it will return null if it's not possible
		/// </summary>
		/// <param name="exportedType"></param>
		/// <param name="requestedType"></param>
		/// <returns></returns>
		public static Type CreateClosedExportTypeFromRequestingType(Type exportedType, Type requestedType)
		{
			if (requestedType.GetTypeInfo().IsInterface)
			{
				return CreateClosedExportTypeFromInterfaceRequestingType(exportedType, requestedType);
			}

			return null;
		}

		private static Type CreateClosedExportTypeFromInterfaceRequestingType(Type exportedType, Type requestedType)
		{
			Type returnType = null;
			TypeInfo exportedTypeInfo = exportedType.GetTypeInfo();
			TypeInfo requestedTypeInfo = requestedType.GetTypeInfo();

			IEnumerable<Type> interfaces =
				exportedTypeInfo.ImplementedInterfaces.Where(x => x.GetTypeInfo().GUID == requestedTypeInfo.GUID);

			foreach (Type @interface in interfaces)
			{
				Dictionary<Type, Type> parameterTypeToRealTypeMap;

				if (TypeMeetRequirements(exportedType, requestedType, @interface, out parameterTypeToRealTypeMap))
				{
					returnType = CreateClosedTypeWithParameterMap(exportedType, parameterTypeToRealTypeMap);

					if (returnType != null)
					{
						break;
					}
				}
			}

			return returnType;
		}

		
		/// <summary>
		/// A helper to check to see if a generic parameter type meets the specified constraints
		/// </summary>
		/// <param name="genericParameterType">The generic parameter type</param>
		/// <param name="exported">The type parameter on the exported class</param>
		/// <returns>True if the type meets the constraints, otherwise false</returns>
		public static bool DoesTypeMeetGenericConstraints(Type genericParameterType, Type exported)
		{
			bool meets = true;
			var constraints = genericParameterType.GetTypeInfo().GetGenericParameterConstraints();

			foreach (Type constraint in constraints)
			{
				if (constraint.GetTypeInfo().IsInterface)
				{
					if (exported.GetTypeInfo().GUID == constraint.GetTypeInfo().GUID)
					{
						continue;
					}

					if (exported.GetTypeInfo().ImplementedInterfaces.Any(x => x.GetTypeInfo().GUID == constraint.GetTypeInfo().GUID))
					{
						continue;
					}

					meets = false;
					break;
				}
				
				if (!constraint.GetTypeInfo().IsAssignableFrom(exported.GetTypeInfo()))
				{
					meets = false;
					break;
				}
			}

			return meets;
		}
		
		/// <summary>
		/// A helper method that checks to see if the type meets the applied constraints on the generic
		/// </summary>
		/// <param name="genericParameterType">
		/// The type parameter on the generic
		/// </param>
		/// <param name="exported">The type parameter on the exported class</param>
		/// <returns>True if the item meets the constraints on the generic, otherwise false</returns>
		private static bool DoesTypeMeetGenericAttributes(Type genericParameterType, Type exported)
		{
			bool meets = true;

			GenericParameterAttributes attributes = genericParameterType.GetTypeInfo().GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

			if (attributes != GenericParameterAttributes.None)
			{
				if (GenericParameterAttributes.None !=
					 (attributes & GenericParameterAttributes.DefaultConstructorConstraint))
				{
					// If the constraint on the generic is something like:
					//   where T: struct
					// The check that is performed is that the GenericParameterAttributes
					// has the DefaultConstructorConstraint set and the code must check to
					// see if both are value types
					if (exported.GetTypeInfo().IsValueType)
					{
						meets = genericParameterType.GetTypeInfo().IsValueType;
					}
					else
					{
						// If the constraint on the generic is something like:
						//  where T: new()
						// The check that is performed is that the GenericParameterAttributes
						// has the DefaultConstructorConstraint set and the exported is not a value
						// type, the type to use to create the generic must have a default
						// constructor
						meets = HasDefaultConstructor(exported);
					}
				}

				// If the constraint on the generic is something like:
				//   where T: class
				// The check that is performed is that the GenericParameterAttributes
				// has the ReferenceTypeConstraint set and the type to use to create the generic
				// must be a class
				if (meets && GenericParameterAttributes.None !=
					 (attributes & GenericParameterAttributes.ReferenceTypeConstraint))
				{
					TypeInfo exportTypeInfo = exported.GetTypeInfo();

					meets = exportTypeInfo.IsClass ||
							  exportTypeInfo.IsInterface ||
							  exportTypeInfo.IsArray;
				}
			}

			return meets;
		}

		/// <summary>
		/// Determines if the type has a default constructor
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <returns>
		/// True if the item has a default constructor, otherwise false
		/// </returns>
		public static bool HasDefaultConstructor(Type type)
		{
			return type.GetTypeInfo().DeclaredConstructors.Any(x => x.IsPublic && !x.GetParameters().Any());
		}

		private static Type CreateClosedTypeWithParameterMap(Type exportedType, Dictionary<Type, Type> parameterTypeToRealTypeMap)
		{
			Type[] genericParameters = exportedType.GetTypeInfo().GenericTypeParameters;
			List<Type> closingTypes = new List<Type>();

			foreach (Type genericParameter in genericParameters)
			{
				Type closeType = parameterTypeToRealTypeMap[genericParameter];

				closingTypes.Add(closeType);
			}

			return exportedType.MakeGenericType(closingTypes.ToArray());
		}

		private static bool TypeMeetRequirements(Type exportedType, Type requestedType, Type @interface, out Dictionary<Type, Type> parameterTypeToRealTypeMap)
		{
			bool returValue = true;
			Type[] interfaceTypes = @interface.GenericTypeArguments;
			Type[] closedRequestedTypes = requestedType.GenericTypeArguments;

			parameterTypeToRealTypeMap = new Dictionary<Type, Type>();

			for (int i = 0; i < interfaceTypes.Length; i++)
			{
				if (interfaceTypes[i].IsGenericParameter)
				{
					if (DoesTypeMeetGenericConstraints(interfaceTypes[i], closedRequestedTypes[i]) &&
					    DoesTypeMeetGenericAttributes(interfaceTypes[i], closedRequestedTypes[i]))
					{
						parameterTypeToRealTypeMap[interfaceTypes[i]] = closedRequestedTypes[i];
					}
					else
					{
						returValue = false;
						break;
					}
				}
				else if(interfaceTypes[i] != closedRequestedTypes[i])
				{
					returValue = false;
					break;
				}
			}

			return returValue;
		}
	}
}
