namespace Grace.UnitTests.Classes.Simple
{
	public interface ISimpleObject
	{
		string TestString { get; }
	}

	public class SimpleObjectA : ISimpleObject
	{
		public string TestString
		{
			get { return "A"; }
		}
	}

	public class SimpleObjectB : ISimpleObject
	{
		public string TestString
		{
			get { return "B"; }
		}
	}

	public class SimpleObjectC : ISimpleObject
	{
		public string TestString
		{
			get { return "C"; }
		}
	}

	public class SimpleObjectD : ISimpleObject
	{
		public string TestString
		{
			get { return "D"; }
		}
	}

	public class SimpleObjectE : ISimpleObject
	{
		public string TestString
		{
			get { return "E"; }
		}
	}
}