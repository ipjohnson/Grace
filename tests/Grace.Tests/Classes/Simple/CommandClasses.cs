using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.Tests.Classes.Simple
{
    public interface ICommand<T>
    {
        void DoSomething(T value);
    }

    public abstract class BaseCommand<T> : ICommand<T>
    {
        public abstract void DoSomething(T value);
    }

    public class CommandA : BaseCommand<int>
    {
        public override void DoSomething(int value)
        {
            
        }
    }

    public class CommandB : BaseCommand<string>
    {
        public override void DoSomething(string value)
        {

        }
    }

    public class CommandC : BaseCommand<double>
    {
        public override void DoSomething(double value)
        {

        }
    }
}
