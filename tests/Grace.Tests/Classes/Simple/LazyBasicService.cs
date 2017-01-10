using System;

namespace Grace.Tests.Classes.Simple
{
    public class LazyBasicService : IBasicService
    {
        private Lazy<IBasicService> _lazyBasic;

        public LazyBasicService(Lazy<IBasicService> lazyBasic)
        {
            _lazyBasic = lazyBasic;
        }

        public int Count
        {
            get { return _lazyBasic.Value.Count; }
            set { _lazyBasic.Value.Count = value; }
        }

        public int TestMethod()
        {
            return _lazyBasic.Value.TestMethod();
        }
    }
}
