using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Grace.Selenium
{
    public abstract class BaseTest
    {
        protected SeleniumContainer _container;
        
        protected void InternalSetup()
        {
            _container = new SeleniumContainer(CreateDriver(), BaseAddress);
        }

        protected void InternalTearDown()
        {
            DestroyDriver();
            _container.Dispose();
        }

        protected abstract string BaseAddress { get; }

        protected abstract IWebDriver CreateDriver();

        protected virtual void DestroyDriver()
        {
            _container.WebDriver.Close();
            _container.WebDriver.Dispose();
        }
    }
}
