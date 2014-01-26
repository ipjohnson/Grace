using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxInjectionContext : IInjectionContext
	{
		private int resolveDepth;

		public FauxInjectionContext()
		{
			MaxResolveDepth = 1000;
		}

		public IInjectionContext Clone()
		{
			return new InjectionContext(DisposalScope, RequestingScope);
		}

		public IDisposalScope DisposalScope { get; set; }

		public IInjectionScope RequestingScope { get; set; }

		public IInjectionTargetInfo TargetInfo { get; set; }

		public object GetExtraData(string dataName)
		{
			return null;
		}

		public void SetExtraData(string dataName, object newValue)
		{
		}

		public T Locate<T>()
		{
			return default(T);
		}

		public object Locate(string name)
		{
			return null;
		}

		public void Export<T>(ExportFunction<T> exportFunction)
		{
		}

		public void Export(Type exportType, ExportActivationDelegate activationDelegate)
		{
		}

		public void Export(string name, ExportActivationDelegate activationDelegate)
		{
		}

		public int MaxResolveDepth { get; set; }

		public void IncrementResolveDepth()
		{
			resolveDepth++;

			if (resolveDepth > MaxResolveDepth)
			{
				throw new DependencyLoopException();
			}
		}

		public void DecrementResolveDepth()
		{
			resolveDepth--;
		}
	}
}