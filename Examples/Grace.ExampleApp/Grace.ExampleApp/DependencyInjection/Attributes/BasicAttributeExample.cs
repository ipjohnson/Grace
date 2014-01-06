using System;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.AttributedExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.Attributes
{
	public class BasicAttributeExample : IExample<AttributeSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()));

			ICalculator calculator = container.Locate<ICalculator>();

			TestAdd(calculator);

			TestSubtract(calculator);

			TestMultiply(calculator);

			TestDivide(calculator);
		}

		private void TestAdd(ICalculator calculator)
		{
			int addValue = calculator.Add(10, 20);

			if (addValue != 30)
			{
				throw new Exception("Value should have been 30");
			}
		}

		private void TestSubtract(ICalculator calculator)
		{
			int subtractValue = calculator.Subtract(30, 10);

			if (subtractValue != 20)
			{
				throw new Exception("Value should have been 20");
			}
		}

		private void TestMultiply(ICalculator calculator)
		{
			int multiplyValue = calculator.Multiply(5, 2);

			if (multiplyValue != 10)
			{
				throw new Exception("Value should have been 10");
			}
		}

		private void TestDivide(ICalculator calculator)
		{
			int divideValue = calculator.Divide(20, 4);

			if (divideValue != 5)
			{
				throw new Exception("Value should have been 5");
			}
		}
	}
}
