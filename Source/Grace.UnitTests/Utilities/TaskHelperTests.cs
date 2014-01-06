using System.Threading.Tasks;
using Grace.Utilities;
using Xunit;

namespace Grace.UnitTests.Utilities
{
	public class TaskHelperTests
	{
		[Fact]
		public void EmptyTaskTest()
		{
			Task emptyTask = TaskHelper.CreateEmptyTask();

			Assert.NotNull(emptyTask);

			Assert.Equal(TaskStatus.RanToCompletion, emptyTask.Status);
		}

		[Fact]
		public void ReturnNullTaskHelper()
		{
			Task<string> nullTask = TaskHelper.NullTask<string>();

			Assert.NotNull(nullTask);

			Assert.Equal(TaskStatus.RanToCompletion, nullTask.Status);

			Assert.Null(nullTask.Result);
		}

		[Fact]
		public void ReturnValueTaskHelper()
		{
			Task<string> stringTask = TaskHelper.ReturnTask("Hello");

			Assert.NotNull(stringTask);

			Assert.Equal(TaskStatus.RanToCompletion, stringTask.Status);

			Assert.Equal("Hello", stringTask.Result);
		}
	}
}