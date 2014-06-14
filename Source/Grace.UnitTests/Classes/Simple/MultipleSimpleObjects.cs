using System;

namespace Grace.UnitTests.Classes.Simple
{
    public class SimpleFilterAttribute : Attribute
    {
        
    }

    [SimpleFilter]
	public interface ISimpleObject
	{
		string TestString { get; }
	}

    [SimpleFilter]
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

    [SimpleFilter]
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

    [SimpleFilter]
	public class SimpleObjectE : ISimpleObject
	{
		public string TestString
		{
			get { return "E"; }
		}
	}
}