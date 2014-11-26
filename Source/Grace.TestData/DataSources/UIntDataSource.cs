using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class UIntDataSource : BaseDataSource<uint>
    {        
        private readonly IRandomDataGeneratorService _randomData;

        public UIntDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            uint minValue = GetConstraintValue(constraints, uint.MinValue, "min", "minvalue");
            uint maxValue = GetConstraintValue(constraints, uint.MaxValue, "max", "maxvalue");

            return _randomData.NextUInt32(minValue, maxValue);
        }
    }
}
