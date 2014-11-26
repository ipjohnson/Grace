using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.DataSources
{
    public class StreetDataSource : BaseDataSource<string>
    {
        private static ImmutableArray<string> _streetNames;

        static StreetDataSource()
        {
            _streetNames = ImmutableArray.From(new []
                                               {
                                                   "First",
                                                   "Second",
                                                   "Third",
                                                   "Fourth",
                                                   "Fifth",
                                                   "Sixth",
                                                   "Seventh",
                                                   "Eighth",
                                                   "Nineth",
                                                   "Tenth",
                                                   "Oak",
                                                   "Pine",
                                                   "Maple",
                                                   "Cedar",
                                                   "Spruce",
                                                   "Lake",
                                                   "Hill",
                                                   "Pond",
                                                   "Court",
                                                   "View",
                                                   "Church",
                                                   "Washington",
                                                   "Main",
                                                   "Broad",
                                                   "Center",
                                                   "Union",
                                                   "Prospect",
                                                   "Highland"
                                               });
        }

        private readonly IRandomDataGeneratorService _randomData;

        public StreetDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _streetNames[_randomData.NextInt(0, _streetNames.Length)];
        }
    }
}
