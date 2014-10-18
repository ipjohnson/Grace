using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using OpenQA.Selenium;

namespace Grace.Selenium
{
    public class SeleniumContainer : DependencyInjectionContainer
    {
        private IWebDriver _webDriver;
        private string _baseAddress;

        public SeleniumContainer(IWebDriver webDriver, string baseAddress = null)
        {
            _webDriver = webDriver;

            _baseAddress = baseAddress;

            SetupContainer();
        }

        public IWebDriver WebDriver
        {
            get { return _webDriver; }
        }

        public virtual string BaseAddress
        {
            get { return _baseAddress; }
            protected set { _baseAddress = value; }
        }

        private void SetupContainer()
        {
            Configure(c => c.ExportInstance((scope, context) => _webDriver));
            
            AddStrategyInspector(new SeleniumExportStrategyInspector(_webDriver));

            AddStrategyInspector(new PropertyInjectionInspector(typeof(IExportLocator)));
            AddStrategyInspector(new PropertyInjectionInspector(typeof(IWebDriver)));
            AddStrategyInspector(new PropertyInjectionInspector(typeof(string),"baseaddress", afterConstructor: true, valueProvider: new FuncValueProvider<string>(() => BaseAddress)));
        }
    }
}
