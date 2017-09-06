namespace Grace.Tests.Classes.Simple
{
    public class CustomBasicService : IBasicService
    {
        public int Count { get; set; }

        public int TestMethod()
        {
            return Count;
        }
    }
}
