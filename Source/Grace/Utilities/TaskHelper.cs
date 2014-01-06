using System.Threading.Tasks;

namespace Grace.Utilities
{
	/// <summary>
	/// Utility methods to help with creating tasks
	/// </summary>
	public static class TaskHelper
	{
		/// <summary>
		/// Creates a new empty task
		/// </summary>
		/// <returns></returns>
		public static Task CreateEmptyTask()
		{
			Task newTask = new Task(() => { });

			newTask.RunSynchronously();

			return newTask;
		}

		/// <summary>
		/// Creates a null task
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Task<T> NullTask<T>() where T : class
		{
			Task<T> newTask = new Task<T>(() => null);

			newTask.RunSynchronously();

			return newTask;
		}

		/// <summary>
		/// Returns a value as a task
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="returnValue"></param>
		/// <returns></returns>
		public static Task<T> ReturnTask<T>(T returnValue)
		{
			Task<T> newTask = new Task<T>(() => returnValue);

			newTask.RunSynchronously();

			return newTask;
		}
	}
}