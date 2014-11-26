using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public abstract class BaseDataSource<T> : IDataSource<T>
    {
        protected T GetConstraintValue<T>(object constraintValue, T defualtValue, params string[] propertyNames)
        {
            PropertyInfo propInfo = null;

            if (constraintValue != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    propInfo = constraintValue.GetType().GetRuntimeProperties().FirstOrDefault(
                                            p => string.Compare(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase) == 0);

                    if (propInfo != null)
                    {
                        break;
                    }
                }
            }

            return propInfo != null ? (T)propInfo.GetValue(constraintValue) : defualtValue;
        }

        public abstract object Next(Type type, string key, IDataRequestContext context, object constraints);
    }
}
