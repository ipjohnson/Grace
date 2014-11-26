using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "Username")]
    public class UsernameDataSource : IDataSource<string>
    {
        private readonly IRandomDataGeneratorService _dataGenerator;
        private readonly IDataSourcePicker _dataSourcePicker;

        public UsernameDataSource(IRandomDataGeneratorService dataGenerator, IDataSourcePicker dataSourcePicker)
        {
            _dataGenerator = dataGenerator;
            _dataSourcePicker = dataSourcePicker;
        }

        public object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            object lastname;
            object firstname;

            context.TryGetValue("lastname", out lastname);
            context.TryGetValue("firstname", out firstname);

            if (lastname == null || firstname == null)
            {
                var lastnameSource = _dataSourcePicker.GetDataSource<string>("Lastname", context, constraints);
                string username = (string)lastnameSource.Next(typeof(string), "Lastname", context, constraints);

                username = Char.ToLower(_dataGenerator.NextChar()) + username.ToLowerInvariant();

                return username;
            }

            return "" + firstname.ToString()[0] + lastname;
        }
    }
}
