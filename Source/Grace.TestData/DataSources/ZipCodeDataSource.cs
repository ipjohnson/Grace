using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "PostalCode")]
    [DataSource(Key = "ZipCode")]
    public class ZipCodeDataSource : BaseDataSource<string>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;

        public ZipCodeDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }
        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _dataGenerator.NextString(StringType.Numeric, 5, 5);
        }
    }
}
