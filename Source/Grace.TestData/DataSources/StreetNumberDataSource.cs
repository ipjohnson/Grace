using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "StreetNumber")]
    [DataSource(Key = "UnitNumber")]
    [DataSource(Key = "AptNumber")]
    [DataSource(Key = "ApartmentNumber")]
    public class StreetNumberDataSource : BaseDataSource<int>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;

        public StreetNumberDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _dataGenerator.NextInt(100, 10000);
        }
    }
}
