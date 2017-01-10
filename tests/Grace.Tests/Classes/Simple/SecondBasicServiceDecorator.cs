namespace Grace.Tests.Classes.Simple
{
    public class SecondBasicServiceDecorator : IBasicService
    {
        private readonly IBasicService _service;

        public SecondBasicServiceDecorator(IBasicService service)
        {
            _service = service;
        }

        public int Count
        {
            get { return _service.Count; }
            set { _service.Count = value; }
        }
        public int TestMethod()
        {
            return _service.TestMethod();
        }
    }
}
