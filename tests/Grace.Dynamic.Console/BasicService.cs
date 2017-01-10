namespace Grace.Dynamic.Console
{
    public interface IBasicService
    {
        int Count { get; }
    }

    public class BasicService : IBasicService
    {
        public int Count { get; }
    }
}
