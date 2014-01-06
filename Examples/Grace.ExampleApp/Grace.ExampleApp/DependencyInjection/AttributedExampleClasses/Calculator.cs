using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;

namespace Grace.ExampleApp.DependencyInjection.AttributedExampleClasses
{
	public interface ICalculator
	{
		int Add(int x, int y);

		int Subtract(int x, int y);

		int Multiply(int x, int y);

		int Divide(int x, int y);
	}

	[Export(typeof(ICalculator))]
	public class Calculator :ICalculator
	{
		private IAdd add;
		private IDivide divide;

		public Calculator(IAdd add)
		{
			this.add = add;
		}

		[Import]
		public ISubtract SubtractClass { get; set; }

		[Import]
		public Lazy<IMultiply> MultiplyLazy { get; set; }

		[Import]
		public void ImportDivide(IDivide divide)
		{
			this.divide = divide;
		}

		public int Add(int x, int y)
		{
			return add.Add(x, y);
		}

		public int Subtract(int x, int y)
		{
			return SubtractClass.Subtract(x, y);
		}

		public int Multiply(int x, int y)
		{
			return MultiplyLazy.Value.Multiply(x, y);
		}

		public int Divide(int x, int y)
		{
			return divide.Divide(x, y);
		}
	}
}
