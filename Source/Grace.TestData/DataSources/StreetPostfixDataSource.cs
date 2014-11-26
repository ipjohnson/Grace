using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "StreetPostfix")]
    public class StreetPostfixDataSource : BaseDataSource<string>
    {
        private static ImmutableArray<string> _streetPostfix;

        static StreetPostfixDataSource()
        {
            _streetPostfix = ImmutableArray.From(new []
                                                 {
                                                     "ALY",
                                                     "AVE",
                                                     "BRK",
                                                     "BYP",
                                                     "DR",
                                                     "GRV",
                                                     "LN",
                                                     "PKY",
                                                     "RD",
                                                     "ST",
                                                     "TPKE"
                                                 });
        }

        private readonly IRandomDataGeneratorService _randomData;

        public StreetPostfixDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _streetPostfix[_randomData.NextInt(0, _streetPostfix.Length)];
        }
    }
}
