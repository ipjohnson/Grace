using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "DOB")]
    [DataSource(Key = "DateOfBirth")]
    public class DateOfBirthDataSource : BaseDataSource<DateTime>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;

        public DateOfBirthDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            DateTime dateOfBirth = DateTime.Now;

            dateOfBirth = dateOfBirth.AddYears(_dataGenerator.NextInt(-102, 0));

            dateOfBirth = dateOfBirth.AddSeconds(_dataGenerator.NextInt(-31536000, 0));

            return dateOfBirth;
        }
    }
}
