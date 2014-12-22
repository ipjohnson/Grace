using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class RandomStringDataSource : BaseDataSource<string>
    {
        private IRandomDataGeneratorService _randomData;

        public RandomStringDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            int minLength = GetConstraintValue(constraints, 5, "min", "minlength");
            int maxLength = GetConstraintValue(constraints, 16, "max", "maxlength");
            
            return _randomData.NextString(minLength, maxLength);
        }
    }
}
