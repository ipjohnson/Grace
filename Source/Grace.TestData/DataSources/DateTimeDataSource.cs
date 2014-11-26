using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class DateTimeDataSource : BaseDataSource<DateTime>
    {
        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return DateTime.Now;
        }
    }
}
