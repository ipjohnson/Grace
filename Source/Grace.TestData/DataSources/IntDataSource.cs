using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class IntDataSource : BaseDataSource<int>
    {
        private readonly IRandomDataGeneratorService _randomData;

        public IntDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            int min = GetConstraintValue(constraints, int.MinValue, "min", "minimum");
            int max = GetConstraintValue(constraints, int.MinValue, "max", "maximum");

            return _randomData.NextInt(min, max);
        }
    }
}
