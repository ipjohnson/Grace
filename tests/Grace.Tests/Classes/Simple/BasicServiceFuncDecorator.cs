using System;

namespace Grace.Tests.Classes.Simple
{
    public class BasicServiceFuncDecorator : IBasicService
    {
        private IBasicService _instance;
        private Func<IBasicService> _func;

        public BasicServiceFuncDecorator(Func<IBasicService> func)
        {
            _func = func;
        }

        public int Count
        {
            get { return Instance.Count; }
            set { Instance.Count = value; }
        }

        public int TestMethod()
        {
            return Instance.TestMethod();
        }

        protected IBasicService Instance => _instance ?? (_instance = _func());
    }
}
