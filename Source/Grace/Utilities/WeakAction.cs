using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.Utilities
{
	/// <summary>
	/// Represents a delegate that will not hold a reference to it's target
	/// </summary>
	public class WeakAction
	{
		private readonly Action<object> executeAction;
		private readonly WeakReference reference;

		/// <summary>
		/// Default Action
		/// </summary>
		/// <param name="action"></param>
		public WeakAction(Action action)
		{
			MethodInfo = action.GetMethodInfo();

			if (action.Target != null)
			{
				reference = new WeakReference(action.Target);

				object cachedDelegate;

				if (!InternalMethodCacheHelper.WeakDelegates.TryGetValue(MethodInfo, out cachedDelegate))
				{
					ParameterExpression param = Expression.Parameter(typeof(object), "target");

					UnaryExpression cast =
						Expression.ConvertChecked(param, action.Target.GetType());

					Expression call = Expression.Call(cast, MethodInfo);

					executeAction =
						Expression.Lambda<Action<object>>(call, param).Compile();

					InternalMethodCacheHelper.WeakDelegates[MethodInfo] = executeAction;
				}
				else
				{
					executeAction = (Action<object>)cachedDelegate;
				}
			}
			else
			{
				executeAction = o => action();
			}
		}

		/// <summary>
		/// True if Target is alive
		/// </summary>
		public bool IsAlive
		{
			get { return reference.IsAlive; }
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
		/// Invoke the action
		/// </summary>
		public void Invoke()
		{
			if (reference != null)
			{
				object target = reference.Target;

				if (executeAction != null && target != null)
				{
					executeAction(target);
				}
			}
			else
			{
				// this is the static case and no object to pass in
				executeAction(null);
			}
		}
	}

	/// <summary>
	/// Weak Action&lt;T&gt;
	/// </summary>
	/// <typeparam name="T">action param</typeparam>
	public class WeakAction<T>
	{
		private readonly Action<object, T> executeAction;
		private readonly WeakReference reference;

		/// <summary>
		/// Default Action
		/// </summary>
		/// <param name="action"></param>
		public WeakAction(Action<T> action)
		{
			MethodInfo = action.GetMethodInfo();

			if (action.Target != null)
			{
				reference = new WeakReference(action.Target);
				object cachedDelegate;

				if (!InternalMethodCacheHelper.WeakDelegates.TryGetValue(MethodInfo, out cachedDelegate))
				{
					ParameterExpression param = Expression.Parameter(typeof(object), "target");

					ParameterExpression tParam = Expression.Parameter(typeof(T), "tParam");

					UnaryExpression cast =
						Expression.ConvertChecked(param, action.Target.GetType());

					Expression call = Expression.Call(cast, MethodInfo, tParam);

					executeAction =
						Expression.Lambda<Action<object, T>>(call, param, tParam).Compile();

					InternalMethodCacheHelper.WeakDelegates[MethodInfo] = executeAction;
				}
				else
				{
					executeAction = (Action<object, T>)cachedDelegate;
				}
			}
			else
			{
				executeAction = (o, t) => action(t);
			}
		}

		/// <summary>
		/// True if Target is alive
		/// </summary>
		public bool IsAlive
		{
			get { return reference.IsAlive; }
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
		/// Invoke the action
		/// </summary>
		public void Invoke(T tParam)
		{
			if (reference != null)
			{
				object target = reference.Target;

				if (executeAction != null && target != null)
				{
					executeAction(target, tParam);
				}
			}
			else
			{
				// this is the static case and no object to pass in
				executeAction(null, tParam);
			}
		}
	}
}