using System;
using System.Collections.Generic;

namespace Grace.Data
{
	/// <summary>
	/// Delegate for handling Complex data changes
	/// </summary>
	/// <param name="complexObject"></param>
	/// <param name="args"></param>
	public delegate void ChildChangedHandler(IComplexObject complexObject, ComplexDataChangedArgs args);

	/// <summary>
	/// Represents a complex data object usually a data model
	/// </summary>
	public interface IComplexObject
	{
		/// <summary>
		/// Parent of the object
		/// </summary>
		IComplexObject Parent { get; }

		/// <summary>
		/// Identity in the parent object (usually property name)
		/// </summary>
		string IdentityInParent { get; }

		/// <summary>
		/// Children of this complex object
		/// </summary>
		IEnumerable<IComplexObject> Children { get; }

		/// <summary>
		/// Is the object dirty
		/// </summary>
		bool IsDirty { get; set; }

		/// <summary>
		/// Set the owning parent of the object
		/// </summary>
		/// <param name="newParent">new parent</param>
		/// <param name="identityInParent">name inside parent</param>
		/// <param name="notifyUponchange">handler to be notified upon change</param>
		/// <returns>returns true if the parent was set</returns>
		bool SetParent(IComplexObject newParent,
			string identityInParent,
			ChildChangedHandler notifyUponchange);

		/// <summary>
		/// Data Changed event
		/// </summary>
		event EventHandler<ComplexDataChangedArgs> DataChanged;
	}
}