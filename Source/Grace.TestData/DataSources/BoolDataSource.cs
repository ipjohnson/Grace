using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class BoolDataSource : BaseDataSource<bool>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;

        public BoolDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }
        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _dataGenerator.NextBool();
        }
    }
}
