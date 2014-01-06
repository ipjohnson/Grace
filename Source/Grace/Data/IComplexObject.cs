using System;
using System.Collections.Generic;

namespace Grace.Data
{
	public delegate void ChildChangedHandler(IComplexObject complexObject, ComplexDataChangedArgs args);

	public interface IComplexObject
	{
		IComplexObject Parent { get; }

		string IdentityInParent { get; }

		IEnumerable<IComplexObject> Children { get; }

		bool IsDirty { get; set; }

		bool SetParent(IComplexObject newParent,
			string identityInParent,
			ChildChangedHandler notifyUponchange);

		event EventHandler<ComplexDataChangedArgs> DataChanged;
	}
}