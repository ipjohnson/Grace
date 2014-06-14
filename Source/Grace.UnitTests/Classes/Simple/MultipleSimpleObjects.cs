using System;

namespace Grace.UnitTests.Classes.Simple
{
    public class SimpleFilterAttribute : Attribute
    {
        
    }

	public interface ISimpleObject
	{
		string TestString { get; }
	}

    [SimpleFilterAttribute]
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

    [SimpleFilterAttribute]
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

    [SimpleFilterAttribute]
	public class SimpleObjectE : ISimpleObject
	{
		public string TestString
		{
			get { return "E"; }
		}
	}
}