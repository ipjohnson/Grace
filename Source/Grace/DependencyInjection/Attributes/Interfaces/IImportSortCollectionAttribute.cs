using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// This interface allows the developer to create a custom attribute that provides an IComparer(T) object to be used to sor
	/// </summary>
	public interface IImportSortCollectionAttribute
	{
		/// <summary>
		/// Provides an IComparer(T) 
		/// </summary>
		/// <param name="attributedType"></param>
		/// <param name="attributedName"></param>
		/// <returns></returns>
		object ProvideComparer(Type attributedType, string attributedName);
	}
}