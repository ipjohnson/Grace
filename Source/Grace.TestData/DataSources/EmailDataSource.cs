using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "Email")]
    [DataSource(Key = "EmailAddress")]
    public class EmailDataSource : BaseDataSource<string>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;
        private readonly IDataSourcePicker _dataSourcePicker;

        public EmailDataSource(IRandomDataGeneratorService dataGenerator, IDataSourcePicker dataSourcePicker)
        {
            _dataGenerator = dataGenerator;
            _dataSourcePicker = dataSourcePicker;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            object lastname = GetConstraintValue(constraints,(object) null, "lastname");
            object firstname = GetConstraintValue(constraints, (object)null, "firstname");

            if (lastname == null)
            {
                var source = _dataSourcePicker.GetDataSource<string>("lastname", context, constraints);

                lastname = source.Next(typeof(string), "Lastname", context, constraints);
            }

            if (firstname == null)
            {
                var source = _dataSourcePicker.GetDataSource<string>("firstname", context, constraints);

                firstname = source.Next(typeof(string), "firstname", context, constraints);
            }

            string domain = GetConstraintValue(constraints, "none.com", "domain");

            return string.Format("{0}.{1}@{2}", firstname, lastname, domain);
        }
    }
}
