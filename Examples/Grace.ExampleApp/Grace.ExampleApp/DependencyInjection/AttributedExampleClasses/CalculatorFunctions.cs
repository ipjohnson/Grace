using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;

namespace Grace.ExampleApp.DependencyInjection.AttributedExampleClasses
{
	public interface IAdd
	{
		int Add(int x, int y);
	}

	public interface ISubtract
	{
		int Subtract(int x, int y);
	}

	public interface IMultiply
	{
		int Multiply(int x, int y);
	}

	public interface IDivide
	{
		int Divide(int x, int y);
	}

	[Export(typeof(IAdd))]
	public class AddClass : IAdd
	{
		public int Add(int x, int y)
		{
			return x + y;
		}
	}

	[Export(typeof(ISubtract))]
	public class SubtractClass : ISubtract
	{
		public int Subtract(int x, int y)
		{
			return x - y;
		}
	}

	[Export(typeof(IMultiply))]
	public class MultiplyClass : IMultiply
	{
		public int Multiply(int x, int y)
		{
			return x * y;
		}
	}

	[Export(typeof(IDivide))]
	public class DivideClass : IDivide
	{
		public int Divide(int x, int y)
		{
			return x / y;
		}
	}
}
