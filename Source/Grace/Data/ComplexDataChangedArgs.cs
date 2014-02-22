using System.ComponentModel;

namespace Grace.Data
{
	/// <summary>
	/// Data change event inside complex object
	/// </summary>
	public class ComplexDataChangedArgs : PropertyChangedEventArgs
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="identityInParent">identity in parent</param>
		/// <param name="propertyName">property name of change</param>
		public ComplexDataChangedArgs(string identityInParent, string propertyName) :
			base(propertyName)
		{
			FullPropertyName = string.Concat(identityInParent, '.', propertyName);
		}

		/// <summary>
		/// Original object that sent the data change event
		/// </summary>
		public IComplexObject OriginalSender { get; set; }

		/// <summary>
		/// Property path to the object that raised the event
		/// </summary>
		public string FullPropertyName { get; set; }

		/// <summary>
		/// Old object value
		/// </summary>
		public object OldValue { get; set; }

		/// <summary>
		/// New object value
		/// </summary>
		public object NewValue { get; set; }
	}
}