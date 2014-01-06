using Grace.DependencyInjection.Impl;
using NSubstitute;

namespace Grace.NSubstitute
{
	/// <summary>
	/// Export strategy that creates it's type using NSubstitute
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NSubstituteExportStrategy<T> : CompiledFuncExportStrategy<T> where T : class
	{
		/// <summary>
		/// Deault constructor that calls NSubstitute to create the instance
		/// </summary>
		public NSubstituteExportStrategy() :
			base((scope, context) => Substitute.For<T>())
		{
		}
	}
}