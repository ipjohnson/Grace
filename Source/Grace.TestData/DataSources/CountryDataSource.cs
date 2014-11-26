using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "Country")]
    public class CountryDataSource : BaseDataSource<string>
    {
        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return "United States";
        }
    }
}
