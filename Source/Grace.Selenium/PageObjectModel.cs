using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using OpenQA.Selenium;

namespace Grace.Selenium
{
    public class PageObjectModel
    {
        public IWebDriver WebDriver { get; set; }

        public IExportLocator Locator { get; set; }

        protected T CreateModel<T>()
        {
            return Locator.Locate<T>();
        }
    }
}
