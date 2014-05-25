using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.Utilities
{
	/// <summary>
	/// Represents a Func(T) that will not hold a reference to the target
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class WeakFunc<TResult>
	{
		private readonly Func<object, TResult> executeAction;
		private readonly WeakReference reference;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="func"></param>
		public WeakFunc(Func<TResult> func)
		{
			MethodInfo = func.GetMethodInfo();

			if (func.Target != null)
			{
				reference = new WeakReference(func.Target);

				object cachedDelegate;

				if (InternalMethodCacheHelper.WeakDelegates.TryGetValue(MethodInfo, out cachedDelegate))
				{
					ParameterExpression param = Expression.Parameter(typeof(object), "target");

					UnaryExpression cast =
						Expression.ConvertChecked(param, func.Target.GetType());

					Expression call = Expression.Call(cast, MethodInfo);

					executeAction =
						Expression.Lambda<Func<object, TResult>>(call, param).Compile();
				}
				else
				{
					executeAction = (Func<object, TResult>)cachedDelegate;
				}
			}
			else
			{
				executeAction = o => func();
			}
		}

		/// <summary>
		/// Is the target alive
		/// </summary>
		public bool IsAlive
		{
			get { return reference == null || reference.IsAlive; }
		}

		/// <summary>
		/// Method info for action
		/// </summary>
		public MethodInfo MethodInfo { get; private set; }

		/// <summary>
		/// Target object
		/// </summary>
		public object Target
		{
			get
			{
				return reference != null ?
						 reference.Target : null;
			}
		}


		/// <summary>
		/// Invoke the Func
		/// </summary>
		/// <returns></returns>
		public TResult Invoke()
		{
			if (reference != null)
			{
				object objectRef = reference.Target;

				if (objectRef != null)
				{
					return executeAction(objectRef);
				}
			}
			else
			{
				// this is the static method case and there is no instance
				return executeAction(null);
			}

			return default(TResult);
		}
	}

	/// <summary>
	/// Represents a Func(T) that will not hold a reference to the target
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <typeparam name="TArg"></typeparam>
	public class WeakFunc<TArg, TResult>
	{
		private readonly Func<object, TArg, TResult> executeAction;
		private readonly WeakReference reference;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="func"></param>
		public WeakFunc(Func<TArg, TResult> func)
		{
			MethodInfo = func.GetMethodInfo();

			if (func.Target != null)
			{
				reference = new WeakReference(func.Target);

				object cachedDelegate;

				if (InternalMethodCacheHelper.WeakDelegates.TryGetValue(MethodInfo, out cachedDelegate))
				{
					ParameterExpression param = Expression.Parameter(typeof(object), "target");

					ParameterExpression tParam = Expression.Parameter(typeof(object), "tArg");

					UnaryExpression cast =
						Expression.ConvertChecked(param, func.Target.GetType());

					Expression call = Expression.Call(cast, MethodInfo, tParam);

					executeAction =
						Expression.Lambda<Func<object, TArg, TResult>>(call, param, tParam).Compile();
				}
				else
				{
					executeAction = (Func<object, TArg, TResult>)cachedDelegate;
				}
			}
			else
			{
				executeAction = (o, t) => func(t);
			}
		}

		/// <summary>
		/// Is the target alive
		/// </summary>
		public bool IsAlive
		{
			get { return reference == null || reference.IsAlive; }
		}

		/// <summary>
		/// Method info for action
		/// </summary>
		public MethodInfo MethodInfo { get; private set; }

		/// <summary>
		/// Target object
		/// </summary>
		public object Target
		{
			get
			{
				return reference != null ?
						 reference.Target : null;
			}
		}


		/// <summary>
		/// Invoke the Func
		/// </summary>
		/// <returns></returns>
		public TResult Invoke(TArg tArg)
		{
			if (reference != null)
			{
				object objectRef = reference.Target;

				if (objectRef != null)
				{
					return executeAction(objectRef, tArg);
				}
			}
			else
			{
				// this is the static method case and there is no instance
				return executeAction(null, tArg);
			}

			return default(TResult);
		}
	}
}