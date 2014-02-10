using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IGenericEntity<T> where T : IGenericEntity<T>
	{
		string SomeProperty { get; }
	}

	public class GenericEntity : IGenericEntity<GenericEntity>
	{
		public string SomeProperty
		{
			get { return "Hello"; }
		}
	}

	public interface IGenericEntityService<T> where T : class, IGenericEntity<T>
	{
		
	}

	public class GenericEntityServuce<T> : IGenericEntityService<T> where T : class, IGenericEntity<T>
	{
		
	}
}
