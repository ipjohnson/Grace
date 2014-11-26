using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "Lastname")]
    [DataSource(Key = "Surname")]
    public class LastnameDataSource : BaseDataSource<string>
    {
        private static readonly ImmutableArray<string> _lastNames;

        static LastnameDataSource()
        {
            _lastNames = ImmutableArray.From(new []
                                             {
                                                 "Johnson",
                                                 "Ray",
                                                 "Archer",
                                                 "Summers",
                                                 "Spencer",
                                                 "Morgan",
                                                 "Freeman",
                                                 "O'Hara",
                                                 "Guster",
                                                 "Vick",
                                                 "Giles",
                                                 "Archer",
                                                 "Kane",
                                                 "Figgis",
                                                 "McDonald",
                                                 "McNabb",
                                                 "Winchester",
                                                 "Despereaux",
                                                 "Tunt",
                                                 "Krieger",
                                                 "Gillette",
                                                 "Jackov",
                                                 "Haris",
                                                 "Rosenberg",
                                                 "Meers",
                                                 "Wilkins",
                                                 "Wood"
                                             });
        }

        private readonly IRandomDataGeneratorService _dataGenerator;

        public LastnameDataSource(IRandomDataGeneratorService dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            string lastname = _lastNames[_dataGenerator.NextInt(0, _lastNames.Length)];

            context["lastname"] = lastname;

            return lastname;
        }
    }
}
