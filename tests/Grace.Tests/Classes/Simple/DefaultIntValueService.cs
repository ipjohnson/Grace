namespace Grace.Tests.Classes.Simple
{
    public interface IDefaultIntValueService
    {
        int Value { get; }
    }

    public class DefaultIntValueService : IDefaultIntValueService
    {
        public const int ConstructorValue = 5;

        public DefaultIntValueService(int value = ConstructorValue)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
