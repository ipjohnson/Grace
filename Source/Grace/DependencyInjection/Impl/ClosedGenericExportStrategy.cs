using System;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Closed generic strategy is used to create closed strategies by the generic strategy
	/// </summary>
	public class ClosedGenericExportStrategy : CompiledInstanceExportStrategy
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType"></param>
		public ClosedGenericExportStrategy(Type exportType) : base(exportType)
		{
		}

		/// <summary>
		/// OVerride equals to compare if to closed generics are equal
		/// </summary>
		/// <param name="obj">object to compare</param>
		/// <returns>compare value</returns>
		public override bool Equals(object obj)
		{
			ClosedGenericExportStrategy strategy = obj as ClosedGenericExportStrategy;

			if (strategy != null &&
			    strategy.CreatingStrategy == CreatingStrategy &&
			    strategy._exportType == _exportType)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets hashcode of activation name
		/// </summary>
		/// <returns>hash code value</returns>
		public override int GetHashCode()
		{
			return ActivationName.GetHashCode();
		}
	}
}