using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "SSN")]
    [DataSource(Key = "SocialSecurityNumber")]
    [DataSource(Key = "GovernmentId")]
    public class SocialSecurityDataSource : BaseDataSource<string>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;

        public SocialSecurityDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            throw new NotImplementedException();
        }
    }
}
