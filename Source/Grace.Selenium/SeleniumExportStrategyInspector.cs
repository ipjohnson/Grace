using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Grace.Selenium
{
    public class SeleniumExportStrategyInspector : IExportStrategyInspector
    {
        private readonly IWebDriver _webDriver;

        public SeleniumExportStrategyInspector(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void Inspect(IExportStrategy exportStrategy)
        {
            exportStrategy.EnrichWithDelegate((scope, context, instance) =>
                                              {
                                                  PageFactory.InitElements(_webDriver,instance);

                                                  return instance;
                                              });
        }
    }
}
