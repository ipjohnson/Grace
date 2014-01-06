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
			if (action.Target != null)
			{
				reference = new WeakReference(action.Target);

				MethodInfo methInfo = action.GetMethodInfo();

				ParameterExpression param = Expression.Parameter(typeof(object), "target");

				UnaryExpression cast =
					Expression.ConvertChecked(param, action.Target.GetType());

				Expression call = Expression.Call(cast, methInfo);

				executeAction =
					Expression.Lambda<Action<object>>(call, param).Compile();
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
}