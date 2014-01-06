using System.ComponentModel;

namespace Grace.Data
{
	public class ComplexDataChangedArgs : PropertyChangedEventArgs
	{
		public ComplexDataChangedArgs(string identityInParent, string propertyName) :
			base(propertyName)
		{
			FullPropertyName = string.Concat(identityInParent, '.', propertyName);
		}

		public IComplexObject OriginalSender { get; set; }

		public string FullPropertyName { get; set; }

		public object OldValue { get; set; }

		public object NewValue { get; set; }
	}
}