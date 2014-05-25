using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxInjectionContext : IInjectionContext
	{
		private int resolveDepth;
		private CurrentInjectionInfo[] currentInjectionInfo;

		public FauxInjectionContext()
		{
			MaxResolveDepth = 1000;
			currentInjectionInfo = new CurrentInjectionInfo[4];
		}

		public IInjectionContext Clone()
		{
			return new FauxInjectionContext();
		}

		public IDisposalScope DisposalScope { get; set; }

		public IInjectionScope RequestingScope { get; set; }

		public IInjectionTargetInfo TargetInfo { get; set; }

		public object Instance { get; set; }

		public object GetExtraData(string dataName)
		{
			return null;
		}

		public void SetExtraData(string dataName, object newValue)
		{
		}

		public object Locate<T>()
		{
			throw new NotImplementedException();
		}

		public object Locate(Type type)
		{
			return null;
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

		/// <summary>
		/// Push a current export strategy onto the stack
		/// </summary>
		/// <param name="exportStrategy">export strategy</param>
		public void PushCurrentInjectionInfo<T>( IExportStrategy exportStrategy)
		{
			if (resolveDepth + 1 > MaxResolveDepth)
			{
				if (TargetInfo != null)
				{
					throw new CircularDependencyDetectedException(TargetInfo.LocateName, TargetInfo.LocateType, this);
				}

				throw new CircularDependencyDetectedException(null, (Type)null, this);
			}

			if (resolveDepth > currentInjectionInfo.Length)
			{
				var temp = new CurrentInjectionInfo[currentInjectionInfo.Length];

				currentInjectionInfo.CopyTo(temp, 0);

				currentInjectionInfo = temp;
			}

			currentInjectionInfo[resolveDepth] = new CurrentInjectionInfo(typeof(T), exportStrategy);

			resolveDepth++;
		}

		/// <summary>
		/// Pop the current export strategy off the stack
		/// </summary>
		public void PopCurrentInjectionInfo()
		{
			resolveDepth--;
		}

		/// <summary>
		/// Injection info all the way up the stack
		/// </summary>
		/// <returns></returns>
		public CurrentInjectionInfo[] GetInjectionStack()
		{
			CurrentInjectionInfo[] returnValue = new CurrentInjectionInfo[resolveDepth];

			for (int i = 0; i < resolveDepth; i++)
			{
				returnValue[i] = currentInjectionInfo[resolveDepth];
			}

			return returnValue;
		}
	}
}