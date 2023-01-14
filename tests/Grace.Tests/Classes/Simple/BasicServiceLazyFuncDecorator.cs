using System;

namespace Grace.Tests.Classes.Simple
{
    public class BasicServiceLazyFuncDecorator : IBasicService
    {
        private readonly Lazy<Func<IBasicService>> _lazyFunc;
        private IBasicService _basicService;

        public BasicServiceLazyFuncDecorator(Lazy<Func<IBasicService>> lazyFunc)
        {
            _lazyFunc = lazyFunc;
        }

        public int Count
        {
            get => Instance.Count; 
            set => Instance.Count = value;
        }

        public int TestMethod()
        {
            return Instance.TestMethod();
        }


        private IBasicService Instance => _basicService ?? (_basicService = _lazyFunc.Value());
    }
}
