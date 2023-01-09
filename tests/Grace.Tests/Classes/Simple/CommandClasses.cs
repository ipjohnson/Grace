namespace Grace.Tests.Classes.Simple
{
    public interface ICommand<in T>
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

    public class OtherCommand : ICommand<int>
    {
        public void DoSomething(int value)
        {
            // No content
        }
    }

    public class LoggingComand<T> : ICommand<T>
    {
        private readonly ICommand<T> _command;
        private readonly IBasicService _basicService;

        public LoggingComand(IBasicService basicService, ICommand<T> command)
        {
            _command = command;
            _basicService = basicService;
        }

        public void DoSomething(T value)
        {
            // Log
            _command.DoSomething(value);
        }
    }

    public class ValidatingCommand<T> : ICommand<T>
    {
        private readonly ICommand<T> _command;
        private readonly IBasicService _basicService;

        public ValidatingCommand( IBasicService basicService, ICommand<T> command)
        {
            _command = command;
            _basicService = basicService;
        }

        public void DoSomething(T value)
        {
            // validate
            _command.DoSomething(value);
        }
    }
}
