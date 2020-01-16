using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.Data.Immutable;
using Grace.DependencyInjection;

namespace Grace.Data
{
    /// <summary>
    /// Helper class for accessing values using reflection
    /// </summary>
    public class ReflectionService
    {
        private static readonly MethodInfo ImmutableTreeAdd =
            typeof(ImmutableHashTree<string, object>).GetRuntimeMethods().First(m => m.Name == nameof(ImmutableHashTree<string, object>.Add));

        private static ImmutableHashTree<Type, PropertyDictionaryDelegate> _propertyDelegates =
            ImmutableHashTree<Type, PropertyDictionaryDelegate>.Empty;
        private static ImmutableHashTree<Type, PropertyDictionaryDelegate> _lowerCasePropertyDelegates =
            ImmutableHashTree<Type, PropertyDictionaryDelegate>.Empty;
        private static ImmutableHashTree<Type, PropertyDictionaryDelegate> _upperCasePropertyDelegates =
            ImmutableHashTree<Type, PropertyDictionaryDelegate>.Empty;

        private static ImmutableHashTree<Type, ExecuteDelegateWithInjection> _executeDelegateWithInjections =
            ImmutableHashTree<Type, ExecuteDelegateWithInjection>.Empty;

        /// <summary>
        /// Delegate for creating dictionaries from object properties
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public delegate ImmutableHashTree<string, object> PropertyDictionaryDelegate(object instance, ImmutableHashTree<string, object> values);

        /// <summary>
        /// Delegate for executing a delegate injecting parameters
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="injectionContext"></param>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public delegate object ExecuteDelegateWithInjection(
            IExportLocatorScope scope, StaticInjectionContext context, IInjectionContext injectionContext,
            Delegate @delegate);

        /// <summary>
        /// Method to get friendly version of a type name for display purposes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeNamespace"></param>
        /// <returns></returns>
        public static string GetFriendlyNameForType(Type type, bool includeNamespace = false)
        {
            if (type.IsConstructedGenericType)
            {
                var builder = new StringBuilder();

                if (includeNamespace)
                {
                    builder.Append(type.Namespace);
                    builder.Append('.');
                }

                CreateFriendlyNameForType(type, builder);

                return builder.ToString();
            }

            return includeNamespace ? type.Namespace + '.' + type.Name : type.Name;
        }

        private static void CreateFriendlyNameForType(Type currentType, StringBuilder builder)
        {
            if (currentType.IsConstructedGenericType)
            {
                var tickIndex = currentType.Name.LastIndexOf('`');
                builder.Append(currentType.Name.Substring(0, tickIndex));
                builder.Append('<');

                var types = currentType.GenericTypeArguments;

                for (var i = 0; i < types.Length; i++)
                {
                    CreateFriendlyNameForType(types[i], builder);

                    if (i + 1 < types.Length)
                    {
                        builder.Append(',');
                    }
                }

                builder.Append('>');
            }
            else
            {
                builder.Append(currentType.Name);
            }
        }

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
                    foreach (var implementedInterface in checkType.GetTypeInfo().ImplementedInterfaces)
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
                    var currentBaseType = checkType;

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
        /// Casing for property names
        /// </summary>
        public enum PropertyCasing
        {
            /// <summary>
            /// Lower case all properties 
            /// </summary>
            Lower,
            /// <summary>
            /// Upper case all properties
            /// </summary>
            Upper,

            /// <summary>
            /// Default casing of property names
            /// </summary>
            Default,
        }

        /// <summary>
        /// Get dictionary of property values from an object
        /// </summary>
        /// <param name="annonymousObject">object to get properties from</param>
        /// <param name="values">collection to add to</param>
        /// <param name="casing">lowercase property names</param>
        /// <returns></returns>
        public static ImmutableHashTree<string, object> GetPropertiesFromObject(object annonymousObject,
            ImmutableHashTree<string, object> values = null, PropertyCasing casing = PropertyCasing.Default)
        {
            values = values ?? ImmutableHashTree<string, object>.Empty;

            if (annonymousObject == null)
            {
                return values;
            }

            if (annonymousObject is Array array)
            {
                var i = 0;

                foreach (var value in array)
                {
                    values = values.Add(i.ToString(), value);
                    i++;
                }

                return values;
            }

            if (annonymousObject is IDictionary<string, object> dictionary)
            {
                return dictionary.Aggregate(values,
                    (v, kvp) => v.Add(kvp.Key, kvp.Value));
            }

            var objectType = annonymousObject.GetType();

            PropertyDictionaryDelegate propertyDelegate = null;

            switch (casing)
            {
                case PropertyCasing.Default:
                    propertyDelegate = _propertyDelegates.GetValueOrDefault(objectType);
                    break;
                case PropertyCasing.Lower:
                    propertyDelegate = _lowerCasePropertyDelegates.GetValueOrDefault(objectType);
                    break;
                case PropertyCasing.Upper:
                    propertyDelegate = _upperCasePropertyDelegates.GetValueOrDefault(objectType);
                    break;
            }

            if (propertyDelegate != null)
            {
                return propertyDelegate(annonymousObject, values);
            }

            propertyDelegate = CreateDelegateForType(objectType, casing);

            switch (casing)
            {
                case PropertyCasing.Default:
                    ImmutableHashTree.ThreadSafeAdd(ref _propertyDelegates, objectType, propertyDelegate);
                    break;
                case PropertyCasing.Lower:
                    ImmutableHashTree.ThreadSafeAdd(ref _lowerCasePropertyDelegates, objectType, propertyDelegate);
                    break;
                case PropertyCasing.Upper:
                    ImmutableHashTree.ThreadSafeAdd(ref _upperCasePropertyDelegates, objectType, propertyDelegate);
                    break;
            }

            return propertyDelegate(annonymousObject, values);
        }

        private static PropertyDictionaryDelegate CreateDelegateForType(Type objectType, PropertyCasing casing)
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
            var currentType = objectType;

            while (currentType != null && currentType != typeof(object))
            {
                foreach (var property in currentType.GetTypeInfo().DeclaredProperties)
                {
                    if (property.CanRead &&
                        !property.GetMethod.IsStatic &&
                        property.GetMethod.IsPublic &&
                        property.GetMethod.GetParameters().Length == 0)
                    {
                        var propertyAccess = Expression.Property(tVariable, property.GetMethod);

                        var propertyCast = Expression.Convert(propertyAccess, typeof(object));

                        var propertyName = property.Name;

                        switch (casing)
                        {
                            case PropertyCasing.Lower:
                                propertyName = propertyName.ToLowerInvariant();
                                break;
                            case PropertyCasing.Upper:
                                propertyName = propertyName.ToUpperInvariant();
                                break;
                        }

                        tree = Expression.Call(tree, ImmutableTreeAdd, Expression.Constant(propertyName), propertyCast,
                            updateDelegate);
                    }
                }

                currentType = currentType.GetTypeInfo().BaseType;
            }

            bodyExpressions.Add(tree);

            var body = Expression.Block(variableList, bodyExpressions);

            return Expression.Lambda<PropertyDictionaryDelegate>(body, inputObject, treeParameter).Compile();
        }

        /// <summary>
        /// Execute delegate by injecting parameters then executing
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="injectionContext"></param>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static object InjectAndExecuteDelegate(IExportLocatorScope scope, StaticInjectionContext context,
            IInjectionContext injectionContext, Delegate @delegate)
        {
            var executeFunc = _executeDelegateWithInjections.GetValueOrDefault(@delegate.GetType());

            if (executeFunc == null)
            {
                executeFunc = CreateExecuteDelegate(@delegate);

                _executeDelegateWithInjections = _executeDelegateWithInjections.Add(@delegate.GetType(), executeFunc);
            }

            return executeFunc(scope, context, injectionContext, @delegate);
        }

        private static ExecuteDelegateWithInjection CreateExecuteDelegate(Delegate @delegate)
        {
            var scopeParameter = Expression.Parameter(typeof(IExportLocatorScope));
            var staticParameter = Expression.Parameter(typeof(StaticInjectionContext));
            var injectionParameter = Expression.Parameter(typeof(IInjectionContext));
            var delegateParameter = Expression.Parameter(typeof(Delegate));

            var method = @delegate.GetType().GetRuntimeMethods().First(m => m.Name == "Invoke");

            var expressions = new List<Expression>();

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof(IExportLocatorScope))
                {
                    expressions.Add(scopeParameter);
                }
                else if (parameter.ParameterType == typeof(StaticInjectionContext))
                {
                    expressions.Add(staticParameter);
                }
                else if (parameter.ParameterType == typeof(IInjectionContext))
                {
                    expressions.Add(injectionParameter);
                }
                else
                {
                    var locateParameter =
                        Expression.Call(scopeParameter,
                            _locateMethod,
                            Expression.Constant(parameter.ParameterType),
                            injectionParameter,
                            Expression.Constant(null, typeof(ActivationStrategyFilter)),
                            Expression.Constant(null, typeof(object)),
                            Expression.Constant(false)
                        );

                    expressions.Add(locateParameter);
                }
            }

            var castExpression = Expression.Convert(delegateParameter, @delegate.GetType());

            Expression call = Expression.Call(castExpression, method, expressions);

            if (!call.Type.IsByRef)
            {
                call = Expression.Convert(call, typeof(object));
            }

            return
                Expression.Lambda<ExecuteDelegateWithInjection>(call, scopeParameter, staticParameter,
                    injectionParameter, delegateParameter).Compile();
        }

        private static readonly MethodInfo _locateMethod = typeof(ILocatorService).GetRuntimeMethod(nameof(ILocatorService.Locate),
            new[] { typeof(Type), typeof(object), typeof(ActivationStrategyFilter), typeof(object), typeof(bool) });
    }
}
