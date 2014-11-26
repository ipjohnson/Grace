using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "AddressLineTwo")]
    [DataSource(Key = "AddressLine2")]
    public class AddressLineTwoDataSource : BaseDataSource<string>
    {
        private readonly IDataSourcePicker _dataSourcePicker;

        public AddressLineTwoDataSource(IDataSourcePicker dataSourcePicker)
        {
            _dataSourcePicker = dataSourcePicker;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return string.Format("Unit {0}",
                _dataSourcePicker.GetDataSource<int>(IntTestData.UnitNumber, context, constraints));
        }
    }
}
