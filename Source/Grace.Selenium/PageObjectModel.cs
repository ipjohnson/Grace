using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.TestData;
using OpenQA.Selenium;

namespace Grace.Selenium
{
    public class PageObjectModel
    {
        public IWebDriver WebDriver { get; set; }

        public IExportLocator Locator { get; set; }

        public ITestDataProvider TestData { get; set; }

        protected T CreateModel<T>()
        {
            return Locator.Locate<T>();
        }

        public virtual FillMethod Fill(string element)
        {
            return Fill(FindAll(element));
        }

        public virtual FillMethod Fill(By elements)
        {
            return Fill(WebDriver.FindElements(elements));
        }

        public virtual FillMethod Fill(IEnumerable<IWebElement> elements)
        {
            return new FillMethod(elements, FillMethodHandler);
        }

        public FormData GetFormData(string element)
        {
            var returnValue = new FormData();
            IWebElement formElement = Find(element);

            if (formElement != null)
            {
                foreach (IWebElement findElement in formElement.FindElements(By.TagName("input")))
                {
                    string key = findElement.GetAttribute("id");

                    if (string.IsNullOrEmpty(key))
                    {
                        key = findElement.GetAttribute("name");
                    }

                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    object value = null;

                    switch (findElement.TagName)
                    {
                        case "input":
                            value = GetInputElementValue(findElement);
                            break;
                        case "select":
                            value = GetSelectElementValue(findElement);
                            break;
                    }

                    if (!returnValue.ContainsKey(key))
                    {
                        returnValue[key] = value;
                    }
                }
            }

            return returnValue;
        }

        private object GetSelectElementValue(IWebElement findElement)
        {
            return null;
        }

        private object GetInputElementValue(IWebElement findElement)
        {
            string typeStr = findElement.GetAttribute("type");

            switch (typeStr)
            {
                case "password":
                case "hidden":
                case "text":
                    return findElement.GetAttribute("value");

                case "checkbox":
                    return findElement.Selected;
                    
            }

            return null;
        }

        private void FillMethodHandler(IEnumerable<IWebElement> elements, object valuesObject, bool throwIfMissingElement)
        {
            foreach (IWebElement webElement in elements)
            {
                foreach (KeyValuePair<string, object> keyValuePair in GetValuesFromObject(valuesObject))
                {
                    string elementName = keyValuePair.Key;
                    IReadOnlyCollection<IWebElement> setElements = null;

                    switch (elementName[0])
                    {
                        case '.':
                            setElements = webElement.FindElements(By.ClassName(elementName.Substring(1)));
                            break;
                        case '#':
                            setElements = webElement.FindElements(By.Id(elementName.Substring(1)));
                            break;
                        default:
                            setElements = webElement.FindElements(By.Id(elementName));

                            if (setElements.Count == 0)
                            {
                                setElements = webElement.FindElements(By.Name(elementName));
                            }
                            break;
                    }

                    SetValueIntoElements(setElements, keyValuePair.Value);
                }
            }
        }

        private void SetValueIntoElements(IReadOnlyCollection<IWebElement> setElements, object value)
        {
            foreach (IWebElement webElement in setElements)
            {
                if (webElement.TagName == "input")
                {
                    webElement.Clear();
                    webElement.SendKeys(value.ToString());
                }
                else if (webElement.TagName == "select")
                {
                    
                }
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetValuesFromObject(object valuesObject)
        {
            Func<object> valueFunc = valuesObject as Func<object>;

            if (valueFunc != null)
            {
                object value = valueFunc();

                if (value == null)
                {
                    throw new Exception("Func must return value");
                }

                return GetValuesFromObject(value);
            }

            if (valuesObject is IEnumerable<KeyValuePair<string, object>>)
            {
                return valuesObject as IEnumerable<KeyValuePair<string, object>>;
            }

            XDocument xDocument = valuesObject as XDocument;

            if (xDocument != null)
            {
                return GetValuesFromXDocument(xDocument);
            }

            return DefaultPropertiesFinder(valuesObject);
        }

        private IEnumerable<KeyValuePair<string, object>> GetValuesFromXDocument(XDocument xDocument)
        {
            yield break;
        }

        private IEnumerable<KeyValuePair<string, object>> DefaultPropertiesFinder(object valuesObject)
        {
            List<KeyValuePair<string,object>> returnList = new List<KeyValuePair<string, object>>();

            foreach (PropertyInfo runtimeProperty in valuesObject.GetType().GetRuntimeProperties())
            {
                if (runtimeProperty.CanRead && 
                    runtimeProperty.GetMethod.IsPublic && 
                    !runtimeProperty.GetMethod.IsStatic && 
                    !runtimeProperty.GetMethod.GetParameters().Any())
                {
                    returnList.Add(
                        new KeyValuePair<string, object>(runtimeProperty.Name,runtimeProperty.GetValue(valuesObject)));
                }
            }

            return returnList;
        }

        public virtual IWebElement Find(string element)
        {
            if (!string.IsNullOrEmpty(element))
            {
                switch (element[0])
                {
                    case '.':
                        return WebDriver.FindElement(By.ClassName(element.Substring(1)));

                    case '#':
                        return WebDriver.FindElement(By.Id(element.Substring(1)));

                    default:
                        IWebElement webElement = WebDriver.FindElement(By.Id(element));

                        if (webElement != null)
                        {
                            return webElement;
                        }

                        return WebDriver.FindElement(By.Name(element));
                }
            }

            return null;
        }

        public virtual IEnumerable<IWebElement> FindAll(string element)
        {
            if (!string.IsNullOrEmpty(element))
            {
                switch (element[0])
                {
                    case '.':
                        return WebDriver.FindElements(By.ClassName(element.Substring(1)));

                    case '#':
                        return WebDriver.FindElements(By.Id(element.Substring(1)));
                        
                    default:
                        var idElements = WebDriver.FindElements(By.Id(element));

                        return idElements.Count > 0 ? idElements : WebDriver.FindElements(By.Name(element));
                }
            }

            return ImmutableArray<IWebElement>.Empty;
        }
    }

    public class FillMethod
    {
        private IEnumerable<IWebElement> _elements;
        private Action<IEnumerable<IWebElement>,object, bool> _fillMethodHandler;

        public FillMethod(IEnumerable<IWebElement> elements, Action<IEnumerable<IWebElement>,object, bool> fillMethodHandler)
        {
            _elements = elements;
            _fillMethodHandler = fillMethodHandler;
        }

        public void With(object valuesObject, bool throwIfMissingElement = true)
        {
            _fillMethodHandler(_elements,valuesObject, throwIfMissingElement);
        }

        public void With(Func<object> valuesObject, bool throwIfMissingElement = true)
        {
            _fillMethodHandler(_elements, valuesObject, throwIfMissingElement);
        }
    }
}
