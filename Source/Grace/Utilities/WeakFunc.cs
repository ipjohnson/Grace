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
			if (func.Target != null)
			{
				reference = new WeakReference(func.Target);

				MethodInfo methInfo = func.GetMethodInfo();

				ParameterExpression param = Expression.Parameter(typeof(object), "target");

				UnaryExpression cast =
					Expression.ConvertChecked(param, func.Target.GetType());

				Expression call = Expression.Call(cast, methInfo);

				executeAction =
					Expression.Lambda<Func<object, TResult>>(call, param).Compile();
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
}