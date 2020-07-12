using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.Utilities
{
    /// <summary>
    /// Static helper class for reflection methods
    /// </summary>
    public static class ReflectionHelper
    {


        /// <summary>
        /// A helper to check to see if a generic parameter type meets the specified constraints
        /// </summary>
        /// <param name="genericParameterType">The generic parameter type</param>
        /// <param name="exported">The type parameter on the exported class</param>
        /// <returns>True if the type meets the constraints, otherwise false</returns>
        public static bool DoesTypeMeetGenericConstraints(Type genericParameterType, Type exported)
        {
            var meets = true;
            var constraints = genericParameterType.GetTypeInfo().GetGenericParameterConstraints();

            foreach (var constraint in constraints)
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
                    if (constraint.IsConstructedGenericType)
                    {
                        if (exported.IsConstructedGenericType)
                        {
                            if (constraint.GenericTypeArguments[0].GetTypeInfo().GUID != Guid.Empty ||
                                constraint.GetGenericTypeDefinition() != exported.GetGenericTypeDefinition())
                            {
                                meets = false;
                                break;
                            }
                        }
                        else
                        {
                            var genericConstraintType = constraint.GetGenericTypeDefinition();
                            var found = false;
                            var parentType = exported.GetTypeInfo().BaseType;

                            while (parentType != typeof(object))
                            {
                                if (parentType.IsConstructedGenericType && 
                                    parentType.GetGenericTypeDefinition() == genericConstraintType)
                                {
                                    found = true;
                                    break;
                                }

                                parentType = parentType.GetTypeInfo().BaseType;
                            }

                            if (!found)
                            {
                                meets = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        meets = false;
                        break;
                    }
                }
            }

            return meets;
        }

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
            else
            {
                return CreateClosedExportTypeFromClassRequestingType(exportedType, requestedType);
            }
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

        /// <summary>
        /// Get the type for a specific MemberInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }

            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            throw new NotSupportedException($"Not supported for MemberInfo of type: {memberInfo.GetType().Name}");
        }


        [Obsolete("This method name has a typo and will be removed at some point. Please use GetMemberType")]
        public static Type GetMemeberType(this MemberInfo memberInfo)
        {
            return GetMemberType(memberInfo);
        }

        private static Type CreateClosedExportTypeFromClassRequestingType(Type exportedType, Type requestedType)
        {
            Type returnType = null;
            var exportedTypeInfo = exportedType.GetTypeInfo();
            var requestedTypeInfo = requestedType.GetTypeInfo();

            if (exportedTypeInfo.GUID == requestedTypeInfo.GUID)
            {
                returnType = requestedType;
            }
            else
            {
                var parentType = exportedTypeInfo.BaseType;

                while (parentType != null && parentType.GetTypeInfo().GUID != requestedTypeInfo.GUID)
                {
                    parentType = parentType.GetTypeInfo().BaseType;
                }

                if (parentType != null)
                {
                    Dictionary<Type, Type> parameterTypeToRealTypeMap;

                    if (TypeMeetRequirements(exportedType, requestedType, parentType, out parameterTypeToRealTypeMap))
                    {
                        returnType = CreateClosedTypeWithParameterMap(exportedType, parameterTypeToRealTypeMap);
                    }
                }
            }

            return returnType;
        }

        private static Type CreateClosedExportTypeFromInterfaceRequestingType(Type exportedType, Type requestedType)
        {
            Type returnType = null;
            var exportedTypeInfo = exportedType.GetTypeInfo();
            var requestedTypeInfo = requestedType.GetTypeInfo();

            var interfaces =
                exportedTypeInfo.ImplementedInterfaces.Where(x => x.GetTypeInfo().GUID == requestedTypeInfo.GUID);

            foreach (var @interface in interfaces)
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
        /// A helper method that checks to see if the type meets the applied constraints on the generic
        /// </summary>
        /// <param name="genericParameterType">
        /// The type parameter on the generic
        /// </param>
        /// <param name="exported">The type parameter on the exported class</param>
        /// <returns>True if the item meets the constraints on the generic, otherwise false</returns>
        private static bool DoesTypeMeetGenericAttributes(Type genericParameterType, Type exported)
        {
            var meets = true;

            var attributes = genericParameterType.GetTypeInfo().GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

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
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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
                    var exportTypeInfo = exported.GetTypeInfo();

                    meets = exportTypeInfo.IsClass ||
                              exportTypeInfo.IsInterface ||
                              exportTypeInfo.IsArray;
                }
            }

            return meets;
        }


        private static Type CreateClosedTypeWithParameterMap(Type exportedType, Dictionary<Type, Type> parameterTypeToRealTypeMap)
        {
            var genericParameters = exportedType.GetTypeInfo().GenericTypeParameters;
            var closingTypes = new List<Type>();

            foreach (var genericParameter in genericParameters)
            {
                var closeType = parameterTypeToRealTypeMap[genericParameter];

                closingTypes.Add(closeType);
            }

            return exportedType.MakeGenericType(closingTypes.ToArray());
        }

        private static bool TypeMeetRequirements(Type exportedType, Type requestedType, Type @interface, out Dictionary<Type, Type> parameterTypeToRealTypeMap)
        {
            var returValue = true;
            var interfaceTypes = @interface.GenericTypeArguments;
            var closedRequestedTypes = requestedType.GenericTypeArguments;

            parameterTypeToRealTypeMap = new Dictionary<Type, Type>();

            for (var i = 0; i < interfaceTypes.Length; i++)
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
                else if (interfaceTypes[i] != closedRequestedTypes[i])
                {
                    returValue = false;
                    break;
                }
            }

            return returValue;
        }

    }
}
