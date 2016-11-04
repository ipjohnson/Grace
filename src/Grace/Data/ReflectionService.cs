using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.Data
{
    /// <summary>
    /// Helper class for accessing values using reflection
    /// </summary>
    public class ReflectionService
    {
        private static readonly MethodInfo ImmutableTreeAdd =
            typeof(ImmutableHashTree<string, object>).GetRuntimeMethods().First(m => m.Name == "Add");

        private static ImmutableHashTree<Type, PropertyDictionaryDelegate> _propertyDelegates =
            ImmutableHashTree<Type, PropertyDictionaryDelegate>.Empty;

        /// <summary>
        /// Delegate for creating dictionaries from object properties
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public delegate ImmutableHashTree<string, object> PropertyDictionaryDelegate(object instance, ImmutableHashTree<string, object> values);


        /// <summary>
        /// Checks to see if checkType is based on baseType
        /// Both inheritance and interface implementation is considered
        /// </summary>
        /// <param name="checkType">check type</param>
        /// <param name="baseType">base type</param>
        /// <returns>true if check type is base type</returns>
        public static bool CheckTypeIsBasedOnAnotherType(Type checkType, Type baseType)
        {
            if (checkType == baseType)
            {
                return true;
            }

            if (baseType.GetTypeInfo().IsInterface)
            {
                if (baseType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    foreach (Type implementedInterface in checkType.GetTypeInfo().ImplementedInterfaces)
                    {
                        if (implementedInterface.IsConstructedGenericType &&
                            implementedInterface.GetTypeInfo().GetGenericTypeDefinition() == baseType)
                        {
                            return true;

                        }
                    }
                }
                else if (checkType.GetTypeInfo().IsInterface)
                {
                    return baseType.GetTypeInfo().IsAssignableFrom(checkType.GetTypeInfo());
                }
                else if (checkType.GetTypeInfo().ImplementedInterfaces.Contains(baseType))
                {
                    return true;
                }
            }
            else
            {
                if (baseType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    Type currentBaseType = checkType;

                    while (currentBaseType != null)
                    {
                        if (currentBaseType.IsConstructedGenericType &&
                            currentBaseType.GetGenericTypeDefinition() == baseType)
                        {
                            return true;
                        }

                        currentBaseType = currentBaseType.GetTypeInfo().BaseType;
                    }
                }
                else
                {
                    return baseType.GetTypeInfo().IsAssignableFrom(checkType.GetTypeInfo());
                }
            }

            return false;
        }


        /// <summary>
        /// Get dictionary of property values from an object
        /// </summary>
        /// <param name="annonymousObject">object to get properties from</param>
        /// <param name="values">collection to add to</param>
        /// <returns></returns>
        public static ImmutableHashTree<string, object> GetPropertiesFromObject(object annonymousObject, ImmutableHashTree<string, object> values = null)
        {
            values = values ?? ImmutableHashTree<string, object>.Empty;

            if (annonymousObject == null)
            {
                return values;
            }

            if (annonymousObject.GetType() == typeof(IDictionary<string, object>))
            {
                return ((IDictionary<string, object>)annonymousObject).Aggregate(values, (v, kvp) => v.Add(kvp.Key, kvp.Value));
            }

            var objectType = annonymousObject.GetType();

            var propertyDelegate = _propertyDelegates.GetValueOrDefault(objectType);

            if (propertyDelegate == null)
            {
                propertyDelegate = CreateDelegateForType(objectType);

                _propertyDelegates = _propertyDelegates.Add(objectType, propertyDelegate);
            }

            return propertyDelegate(annonymousObject, values);
        }

        private static PropertyDictionaryDelegate CreateDelegateForType(Type objectType)
        {
            // the parameter to call the method on
            var inputObject = Expression.Parameter(typeof(object), "inputObject");

            var treeParameter = Expression.Parameter(typeof(ImmutableHashTree<string, object>), "tree");

            // loval variable of type declaringType
            var tVariable = Expression.Variable(objectType);

            // cast the input object to be declaring type
            Expression castExpression = Expression.Convert(inputObject, objectType);

            // assign the cast value to the tVaraible variable
            Expression assignmentExpression = Expression.Assign(tVariable, castExpression);
            

            // keep a list of the variable we declare for use when we define the body
            var variableList = new List<ParameterExpression> { tVariable };

            var bodyExpressions = new List<Expression> { assignmentExpression };

            var updateDelegate =
                Expression.Constant(
                    new ImmutableHashTree<string, object>.UpdateDelegate((oldValue, newValue) => newValue));

            Expression tree = treeParameter;

            foreach (var property in objectType.GetTypeInfo().DeclaredProperties)
            {
                if (property.CanRead &&
                  !property.GetMethod.IsStatic &&
                   property.GetMethod.IsPublic &&
                   property.GetMethod.GetParameters().Length == 0)
                {
                    var propertyAccess = Expression.Property(tVariable, property.GetMethod);

                    var propertyCast = Expression.Convert(propertyAccess, typeof(object));

                    tree = Expression.Call(tree, ImmutableTreeAdd, Expression.Constant(property.Name), propertyCast, updateDelegate);
                }
            }

            bodyExpressions.Add(tree);

            var body = Expression.Block(variableList, bodyExpressions);

            return Expression.Lambda<PropertyDictionaryDelegate>(body, inputObject, treeParameter).Compile();
        }
    }
}
