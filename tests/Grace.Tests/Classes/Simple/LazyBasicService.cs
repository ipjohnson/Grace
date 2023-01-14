using System;

namespace Grace.Tests.Classes.Simple
{
    public class LazyBasicService : IBasicService
    {
        private readonly Lazy<IBasicService> _lazyBasic;

        public LazyBasicService(Lazy<IBasicService> lazyBasic)
        {
            _lazyBasic = lazyBasic;
        }

        public int Count
        {
            get => _lazyBasic.Value.Count;
            set => _lazyBasic.Value.Count = value;
        }

        public int TestMethod()
        {
            return _lazyBasic.Value.TestMethod();
        }
    }
}
