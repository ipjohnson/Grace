namespace Grace.Tests.Classes.Simple
{
    public interface IMultipleService
    {
        int Count { get; }
    }

    public class MultipleService1 : IMultipleService
    {
        public int Count { get; set; }
    }

    public class MultipleService2 : IMultipleService
    {
        public int Count { get; set; }
    }

    public class MultipleService3 : IMultipleService
    {
        public int Count { get; set; }
    }

    public class MultipleService4 : IMultipleService
    {
        public int Count { get; set; }
    }

    public class MultipleService5 : IMultipleService
    {
        public int Count { get; set; }
    }

}
