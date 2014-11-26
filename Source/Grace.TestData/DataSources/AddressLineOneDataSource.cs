using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "AddressLine1")]
    [DataSource(Key = "AddressLineOne")]
    public class AddressLineOneDataSource : BaseDataSource<string>
    {
        private IDataSourcePicker _dataSourcePicker;

        public AddressLineOneDataSource(IDataSourcePicker dataSourcePicker)
        {
            _dataSourcePicker = dataSourcePicker;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return string.Format("{0} {1} {2} {3}",
                _dataSourcePicker.GetDataSource<int>(IntTestData.StreetNumber, context, constraints),
                _dataSourcePicker.GetDataSource<string>(StringTestData.StreetPrefix, context, constraints),
                _dataSourcePicker.GetDataSource<string>(StringTestData.Street, context, constraints),
                _dataSourcePicker.GetDataSource<string>(StringTestData.StreetPostfix, context, constraints));
        }
    }
}
