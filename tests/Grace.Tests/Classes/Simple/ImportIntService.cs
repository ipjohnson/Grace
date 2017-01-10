namespace Grace.Tests.Classes.Simple
{
    public interface IImportIntService
    {
        int Value { get; }
    }

    public class ImportIntService : IImportIntService
    {
        public ImportIntService(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
