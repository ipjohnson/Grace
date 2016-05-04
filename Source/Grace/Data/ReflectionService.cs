using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grace.Utilities;

namespace Grace.Data
{
	/// <summary>
	/// Service creates delegates that can be used to access properties off objects at runtime
	/// </summary>
	public class ReflectionService : IReflectionService
	{
		private static readonly ConstructorInfo _exceptionConstructor = null;
		private static readonly PropertyInfo _objectArrayIndex = null;
        private static readonly MethodInfo _dictionaryAdd = null;

		private readonly SafeDictionary<string, GetPropertyDelegate> _getPropertyAccessors;
		private readonly SafeDictionary<string, SetPropertyDelegate> _setPropertyAccessors;
		private readonly SafeDictionary<string, CallMethodDelegate> _callAccessors;
        private readonly SafeDictionary<Guid, PropertyDictionaryDelegate> _propertyDictionaryAccessors;

		static ReflectionService()
		{
			_objectArrayIndex = typeof(IList).GetTypeInfo().GetDeclaredProperty("Item");

			foreach (ConstructorInfo declaredConstructor in typeof(Exception).GetTypeInfo().DeclaredConstructors)
			{
				if (declaredConstructor.GetParameters().Count() == 1 &&
					 declaredConstructor.GetParameters().FirstOrDefault().ParameterType == typeof(string))
				{
					_exceptionConstructor = declaredConstructor;
				}
			}

            _dictionaryAdd = typeof(Dictionary<string, object>).GetTypeInfo().GetDeclaredMethod("Add");
        }

		/// <summary>
		/// Default constructor
		/// </summary>
		public ReflectionService()
		{
			_getPropertyAccessors =
				new SafeDictionary<string, GetPropertyDelegate>();

			_setPropertyAccessors =
				new SafeDictionary<string, SetPropertyDelegate>();

			_callAccessors =
				new SafeDictionary<string, CallMethodDelegate>();

            _propertyDictionaryAccessors =
                new SafeDictionary<Guid, PropertyDictionaryDelegate>();
		}

		/// <summary>
		/// Gets a named property value from an object
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name (can be nested A.B.C.D)</param>
		/// <returns>property value</returns>
		public object GetPropertyValue(object valueObject, string propertyName)
		{
			return GetPropertyValue(valueObject, propertyName, null, false);
		}

		/// <summary>
		/// Gets a named property value from an object
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name (can be nested A.B.C.D)</param>
		/// <param name="index">index for final property</param>
		/// <param name="throwIfPathMissing">throw an exception if any part of the path is missing</param>
		/// <returns>property value</returns>
		public object GetPropertyValue(object valueObject, string propertyName, object index, bool throwIfPathMissing)
		{
			if (valueObject == null)
			{
				throw new ArgumentNullException("valueObject");
			}

			if (String.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			Type indexType = null;

			if (index != null)
			{
				indexType = index.GetType();
			}

			GetPropertyDelegate getAction = CreateGetPropertyDelegate(valueObject.GetType(), propertyName, indexType);

			if (getAction != null)
			{
				return getAction(valueObject, index, throwIfPathMissing);
			}

			throw new Exception("Could not create property delegate");
		}

		/// <summary>
		/// Sets a value into a named Property
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name to fetch</param>
		/// <param name="newValue"></param>
		public bool SetPropertyValue(object valueObject, string propertyName, object newValue)
		{
			return SetPropertyValue(valueObject, propertyName, newValue, null, true);
		}

		/// <summary>
		/// Sets a value into a named Property
		/// </summary>
		/// <param name="valueObject">target object</param>
		/// <param name="propertyName">property name to fetch</param>
		/// <param name="newValue"></param>
		/// <param name="index"></param>
		/// <param name="createIfPathMissing"></param>
		public bool SetPropertyValue(object valueObject,
											  string propertyName,
											  object newValue,
											  object index,
											  bool createIfPathMissing)
		{
			if (valueObject == null)
			{
				throw new ArgumentNullException("valueObject");
			}

			if (String.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			Type indexType = null;

			if (index != null)
			{
				indexType = index.GetType();
			}

			SetPropertyDelegate setAction = CreateSetPropertyDelegate(valueObject.GetType(), propertyName, indexType);

			if (setAction != null)
			{
				setAction(valueObject, newValue, index, createIfPathMissing);

				return true;
			}

			throw new Exception("Could not create set for property " + propertyName);
		}

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
		public object CallMethod(object target, string methodName, bool throwIfPathMissing, params object[] parameters)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			if (String.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}

			CallMethodDelegate callMethod = null;
			string searchMethodName = methodName;

			if (parameters != null && parameters.Length > 0)
			{
				searchMethodName += "|" + parameters.Length;
			}

			if (!_callAccessors.TryGetValue(searchMethodName, out callMethod))
			{
				ParameterExpression valueObjectParameter = Expression.Parameter(typeof(object), "valueObject");
				ParameterExpression callParameters = Expression.Parameter(typeof(object[]), "callParameters");
				ParameterExpression throwParameter = Expression.Parameter(typeof(bool), "throwIfMissing");
				ParameterExpression returnValue = Expression.Variable(typeof(object), "returnValue");

				ParameterExpression castValue = Expression.Variable(target.GetType(), "castValue");
				Expression castExpression =
					Expression.Assign(castValue,
											Expression.Convert(valueObjectParameter, target.GetType()));

				Expression accessBlock
					= CreateAccessPathCode(castValue,
												  throwParameter,
												  target.GetType(),
												  methodName,
												  (type, name, expression) =>
												  {
													  MethodInfo callMethodInfo = null;

													  foreach (MethodInfo declaredMethod in
														  type.GetTypeInfo().GetDeclaredMethods(name))
													  {
														  if (parameters != null)
														  {
															  if (declaredMethod.GetParameters().Length == parameters.Length)
															  {
																  callMethodInfo = declaredMethod;
																  break;
															  }
														  }
														  else if (declaredMethod.GetParameters().Length == 0)
														  {
															  callMethodInfo = declaredMethod;
															  break;
														  }
													  }

													  if (callMethodInfo == null)
													  {
														  throw new Exception(
															  String.Format("Could not find method {0} on {1}",
																				 name,
																				 type.FullName));
													  }

													  Expression callExpression = null;

													  if (parameters != null && parameters.Length > 0)
													  {
														  int parmeterIndex = 0;
														  List<Expression> callParametersList = new List<Expression>();

														  foreach (var parameterInfo in callMethodInfo.GetParameters())
														  {
															  Expression callParameter =
																  Expression.Convert(
																	  Expression.Property(callParameters,
																								 _objectArrayIndex,
																								 Expression.Constant(parmeterIndex)),
																	  parameterInfo.ParameterType);

															  callParametersList.Add(callParameter);

															  parmeterIndex++;
														  }

														  callExpression =
															  Expression.Call(expression, callMethodInfo, callParametersList);
													  }
													  else
													  {
														  callExpression =
															  Expression.Call(expression, callMethodInfo);
													  }

													  if (callMethodInfo.ReturnType.Name == "Void")
													  {
														  return callExpression;
													  }
													  else
													  {
														  return Expression.Assign(
															  returnValue,
															  Expression.Convert(
																  callExpression,
																  typeof(object)));
													  }
												  });

				BlockExpression delegatebod =
					Expression.Block(new[] { castValue, returnValue },
										  castExpression,
										  accessBlock,
										  returnValue);

				callMethod = Expression.Lambda<CallMethodDelegate>(
					delegatebod, valueObjectParameter, throwParameter, callParameters).
												Compile();

				_callAccessors[searchMethodName] = callMethod;
			}

			return callMethod(target, throwIfPathMissing, parameters);
		}

		/// <summary>
		/// Creates a new delegate that can be used to access a property in an object by property name
		/// </summary>
		/// <param name="instanceType">object type to target</param>
		/// <param name="propertyName">property name (can be dotted form A.B.C)</param>
		/// <returns>new property delegate</returns>
		public GetPropertyDelegate CreateGetPropertyDelegate(Type instanceType, string propertyName, Type indexType)
		{
			GetPropertyDelegate getAction = null;
			string methodName = instanceType.FullName + "." + propertyName;

			if (indexType != null)
			{
				methodName += "|" + indexType.FullName;
			}

			if (!_getPropertyAccessors.TryGetValue(methodName, out getAction))
			{
				ParameterExpression valueObjectParameter = Expression.Parameter(typeof(object), "valueObject");
				ParameterExpression indexParameter = Expression.Parameter(typeof(object), "index");
				ParameterExpression throwParameter = Expression.Parameter(typeof(bool), "throwIfMissing");
				ParameterExpression returnValueExpression = Expression.Variable(typeof(object), "returnValue");

				ParameterExpression castValue = Expression.Variable(instanceType, "castValue");
				Expression castExpression =
					Expression.Assign(castValue,
											Expression.Convert(valueObjectParameter, instanceType));

				Expression accessBlock
					= CreateAccessPathCode(castValue,
												  throwParameter,
												  instanceType,
												  propertyName,
												  (type, name, expression) =>
												  {
													  Expression returnValue = null;
													  Type fieldOrPropertyType = CreateGetPropertyOrFieldType(type, name);

													  if (indexType != null)
													  {
														  PropertyInfo propertyInfo =
															  type.GetTypeInfo().GetDeclaredProperty(propertyName);

														  if (propertyInfo != null)
														  {
															  returnValue = Expression.Assign(
																  returnValueExpression,
																  Expression.Convert(
																	  Expression.Property(expression,
																								 propertyInfo,
																								 Expression.Convert(indexParameter, indexType)),
																	  typeof(object)));
														  }
														  else
														  {
															  throw new Exception(
																  String.Format("Could not find property {0} on type {1}",
																					 type.FullName,
																					 name));
														  }
													  }
													  else
													  {
														  returnValue = Expression.Assign(
															  returnValueExpression,
															  Expression.Convert(
																  Expression.PropertyOrField(expression, name), typeof(object)));
													  }

													  return returnValue;
												  });

				BlockExpression returnBlock =
					Expression.Block(new[] { castValue, returnValueExpression },
										  castExpression,
										  accessBlock,
										  returnValueExpression);

				getAction = Expression.Lambda<GetPropertyDelegate>(
					returnBlock, valueObjectParameter, indexParameter, throwParameter).
											  Compile();

				_getPropertyAccessors[methodName] = getAction;
			}

			return getAction;
		}

		/// <summary>
		/// Creates a new delegate that be used to set a property on an object by property name
		/// </summary>
		/// <param name="instanceType">object type to target</param>
		/// <param name="propertyName">property name (can be dotted form A.B.C)</param>
		/// <param name="indexType">type of property used for indexing</param>
		/// <returns>new property delegate</returns>
		public SetPropertyDelegate CreateSetPropertyDelegate(Type instanceType, string propertyName, Type indexType)
		{
			if (instanceType == null)
			{
				throw new ArgumentNullException("instanceType");
			}

			if (String.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}

			SetPropertyDelegate setAction = null;
			string methodName = instanceType.FullName + "." + propertyName;

			if (indexType != null)
			{
				methodName += "|" + indexType.FullName;
			}

			if (!_setPropertyAccessors.TryGetValue(methodName, out setAction))
			{
				ParameterExpression valueObjectParameter = Expression.Parameter(typeof(object), "valueObject");
				ParameterExpression newValueParameter = Expression.Parameter(typeof(object), "newValueParameter");
				ParameterExpression indexParameter = Expression.Parameter(typeof(object), "index");
				ParameterExpression createParameter = Expression.Parameter(typeof(bool), "createIfMissing");

				ParameterExpression castValue = Expression.Variable(instanceType, "castValue");
				Expression castExpression =
					Expression.Assign(castValue,
											Expression.Convert(valueObjectParameter, instanceType));

				Expression accessBlock
					= CreateAccessPathCode(castValue,
												  createParameter,
												  instanceType,
												  propertyName,
												  (type, name, expression) =>
												  {
													  Expression returnValue = null;

													  if (indexType != null)
													  {
														  PropertyInfo propertyInfo =
															  type.GetTypeInfo().GetDeclaredProperty(propertyName);

														  if (propertyInfo != null)
														  {
															  returnValue = Expression.Assign(
																  Expression.Property(expression,
																							 propertyInfo,
																							 Expression.Convert(indexParameter, indexType)),
																  Expression.Convert(newValueParameter, propertyInfo.PropertyType));
														  }
														  else
														  {
															  throw new Exception(
																  String.Format("Could not find property {0} on type {1}",
																					 type.FullName,
																					 name));
														  }
													  }
													  else
													  {
														  Type fieldOrPropertyType = CreateGetPropertyOrFieldType(type, name);

														  returnValue = Expression.Assign(
															  Expression.PropertyOrField(expression, name),
															  Expression.Convert(newValueParameter, fieldOrPropertyType));
													  }

													  return returnValue;
												  });

				BlockExpression delegatebod =
					Expression.Block(new[] { castValue }, castExpression, accessBlock);

				setAction = Expression.Lambda<SetPropertyDelegate>(
					delegatebod, valueObjectParameter, newValueParameter, indexParameter, createParameter).
											  Compile();

				_setPropertyAccessors[methodName] = setAction;
			}

			return setAction;
		}

		/// <summary>
		/// Gets property info for a dotted property
		/// </summary>
		/// <param name="baseType">type</param>
		/// <param name="propertyString">property string</param>
		/// <returns></returns>
		public static PropertyInfo GetPropertyInfo(Type baseType, string propertyString)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}

			if (String.IsNullOrEmpty(propertyString))
			{
				throw new ArgumentNullException("propertyString");
			}

			int periodIndex = propertyString.IndexOf('.');

			if (periodIndex > 0)
			{
				string newPropertyString = propertyString.Substring(0, periodIndex);
				string theRest = propertyString.Substring(periodIndex + 1);

				PropertyInfo propertyInfo =
					baseType.GetTypeInfo().GetDeclaredProperty(newPropertyString);

				if (propertyInfo != null)
				{
					return GetPropertyInfo(propertyInfo.PropertyType, theRest);
				}
			}
			else
			{
				PropertyInfo propertyInfo =
					baseType.GetTypeInfo().GetDeclaredProperty(propertyString);

				if (propertyInfo != null)
				{
					return propertyInfo;
				}
			}

			return null;
		}

		internal static string GetPropertyAccessPath(Expression<Func<object, object>> method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			MemberExpression memberExpr = null;

			if (method.Body.NodeType == ExpressionType.Convert)
			{
				memberExpr =
					((UnaryExpression)method.Body).Operand as MemberExpression;
			}
			else if (method.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpr = method.Body as MemberExpression;
			}

			if (memberExpr == null)
			{
				throw new ArgumentException("method");
			}

			StringBuilder builder = new StringBuilder();

			while (memberExpr != null)
			{
				if (builder.Length != 0)
				{
					builder.Insert(0, '.');
				}

				builder.Insert(0, memberExpr.Member.Name);

				memberExpr = memberExpr.Expression as MemberExpression;
			}

			return builder.ToString();
		}

		public static object ExternalCreateAccessPathCode(object valueParameter,
																		  object throwParameter,
																		  Type objectType,
																		  string propertyName,
																		  object createAction)
		{
			return CreateAccessPathCode(valueParameter as ParameterExpression,
												 throwParameter as ParameterExpression,
												 objectType,
												 propertyName,
												 createAction as Func<Type, string, Expression, Expression>);
		}

		private static Expression CreateAccessPathCode(ParameterExpression valueParameter,
																	  ParameterExpression throwParameter,
																	  Type objectType,
																	  string propertyName,
																	  Func<Type, string, Expression, Expression> createAction)
		{
			Expression returnExpression = null;
			int firstPeriod = propertyName.IndexOf('.');

			if (firstPeriod > 0)
			{
				string currentPropertyName = propertyName.Substring(0, firstPeriod);
				string theRest = propertyName.Substring(firstPeriod + 1);
				Type propertyOrFieldType = CreateGetPropertyOrFieldType(objectType, currentPropertyName);

				ParameterExpression newValue = Expression.Variable(propertyOrFieldType);

				Expression assignExpression = Expression.Assign(newValue,
																				Expression.PropertyOrField(valueParameter, currentPropertyName));

				if (!propertyOrFieldType.GetTypeInfo().IsValueType)
				{
					Expression recurse =
						CreateAccessPathCode(newValue,
													throwParameter,
													propertyOrFieldType,
													theRest,
													createAction);

					Expression ifExpression = Expression.IfThenElse(
						Expression.NotEqual(newValue, Expression.Constant(null)),
						recurse,
						Expression.IfThen(Expression.IsTrue(throwParameter),
												Expression.Throw(
													Expression.New(
														_exceptionConstructor,
														Expression.Constant(
															String.Format("Could not find property {1} on type {0}",
																			  objectType.FullName,
																			  currentPropertyName))))));

					returnExpression =
						Expression.Block(new[] { newValue }, new[] { assignExpression, ifExpression });
				}
				else
				{
					Expression recurse = CreateAccessPathCode(newValue,
																			throwParameter,
																			propertyOrFieldType,
																			theRest,
																			createAction);

					returnExpression =
						Expression.Block(new[] { newValue }, new[] { assignExpression, recurse });
				}
			}
			else
			{
				returnExpression = createAction(objectType, propertyName, valueParameter);
			}

			return returnExpression;
		}

		private static Type CreateGetPropertyOrFieldType(Type objectType, string propertyName)
		{
			Type propertyOrFieldType = null;

			PropertyInfo propertyInfo =
				objectType.GetTypeInfo().GetDeclaredProperty(propertyName);

			if (propertyInfo == null)
			{
				FieldInfo field = objectType.GetTypeInfo().GetDeclaredField(propertyName);

				if (field != null)
				{
					propertyOrFieldType = field.FieldType;
				}
				else
				{
					throw new Exception(
						String.Format("Could not find property {0} on type {1}",
										  objectType.FullName,
										  propertyName));
				}
			}
			else
			{
				propertyOrFieldType = propertyInfo.PropertyType;
			}

			return propertyOrFieldType;
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

	    public static string GetFriendlyNameForType(Type type, bool includeNamespace = false)
	    {
	        if (type.IsConstructedGenericType)
	        {
	            StringBuilder builder = new StringBuilder();

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
                int tickIndex = currentType.Name.LastIndexOf('`');
	            builder.Append(currentType.Name.Substring(0, tickIndex));
	            builder.Append('<');

                Type[] types = currentType.GenericTypeArguments;

	            for (int i = 0; i < types.Length; i++)
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

        public IDictionary<string, object> GetPropertiesFromObject(object annonymousObject)
        {
            if(annonymousObject == null)
            {
                return new Dictionary<string, object>();
            }

            PropertyDictionaryDelegate propertyDelegate;
            var objectType = annonymousObject.GetType();

            if(!_propertyDictionaryAccessors.TryGetValue(objectType.GetTypeInfo().GUID,out propertyDelegate))
            {
                propertyDelegate = CreateDelegateForType(objectType);

                _propertyDictionaryAccessors[objectType.GetTypeInfo().GUID] = propertyDelegate;
            }

            return propertyDelegate(annonymousObject);
        }

        private PropertyDictionaryDelegate CreateDelegateForType(Type objectType)
        {
            // the parameter to call the method on
            ParameterExpression inputObject = Expression.Parameter(typeof(object), "inputObject");

            // loval variable of type declaringType
            ParameterExpression tVariable = Expression.Variable(objectType);


            // cast the input object to be declaring type
            Expression castExpression = Expression.Convert(inputObject, objectType);

            // assign the cast value to the tVaraible variable
            Expression assignmentExpression = Expression.Assign(tVariable, castExpression);

            ParameterExpression dictionary = Expression.Variable(typeof(Dictionary<string, object>));

            // keep a list of the variable we declare for use when we define the body
            List<ParameterExpression> variableList = new List<ParameterExpression> { tVariable, dictionary };

            List<Expression> bodyExpressions = new List<Expression> { assignmentExpression };
            
            Expression dictionaryAssignment = Expression.Assign(dictionary,  Expression.New(typeof(Dictionary<string, object>)));

            bodyExpressions.Add(dictionaryAssignment);

            foreach (var property in objectType.GetTypeInfo().DeclaredProperties)
            {                
                if(property.CanRead && 
                  !property.GetMethod.IsStatic && 
                   property.GetMethod.IsPublic && 
                   property.GetMethod.GetParameters().Count() == 0)
                {
                    var propertyAccess = Expression.Property(tVariable, property.GetMethod);

                    var propertyCast = Expression.Convert(propertyAccess, typeof(object));

                    var addExpression = Expression.Call(dictionary, _dictionaryAdd, Expression.Constant(property.Name), propertyCast);

                    bodyExpressions.Add(addExpression);
                }
            }

            bodyExpressions.Add(dictionary);

            BlockExpression body = Expression.Block(variableList, bodyExpressions);

            return Expression.Lambda<PropertyDictionaryDelegate>(body, inputObject).Compile();
        }
    }
}
