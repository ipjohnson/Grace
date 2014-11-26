using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.DataSources
{
    [DataSource(Key = "Firstname")]
    [DataSource(Key = "Middlename")]
    public class FirstnameDataSource : BaseDataSource<string>
    {
        static FirstnameDataSource()
        {
            SetupBoyNames();

            SetupGirlNames();
        
        }

        private static ImmutableArray<string> _boyNames;
        private static ImmutableArray<string> _girlNames;
        private IRandomDataGeneratorService _generatorService;

        public FirstnameDataSource(IRandomDataGeneratorService generatorService)
        {
            _generatorService = generatorService;

            }


        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            string sex = GetConstraintValue(constraints, "both", "sex", "gender");

            bool isGirl = false;

            switch (sex.ToLowerInvariant())
            {
                case "m":
                case "male":
                case "b":
                case "boy":
                    isGirl = false;
                    break;

                case "f":
                case "female":
                case "g":
                case "girl":
                    isGirl = true;
                    break;
                    
                default:
                    isGirl = _generatorService.NextBool();
                    break;
            }

            return isGirl ? 
                   _girlNames[_generatorService.NextInt(0, _girlNames.Length)] : 
                   _boyNames[_generatorService.NextInt(0, _boyNames.Length)];

        }


        private static void SetupGirlNames()
        {
            _girlNames = ImmutableArray.From(new[]
                                             {
                                                 "Sophia",
                                                 "Emma",
                                                 "Olivia",
                                                 "Isabella",
                                                 "Mia",
                                                 "Zoey",
                                                 "Emily",
                                                 "Chloe",
                                                 "Sara",
                                                 "Tanya",
                                                 "Jennifer",
                                                 "Kate",
                                                 "Kerry",
                                                 "Kim",
                                                 "Kathy",
                                                 "Glenda",
                                                 "Hannah",
                                                 "Madison",
                                                 "Devin",
                                                 "Kelsey",
                                                 "Lila",
                                                 "Lindsey",
                                                 "Sophie",
                                                 "Grace",
                                                 "Brooklyn",
                                                 "Melissa",
                                                 "Buffy",
                                                 "Faith",
                                                 "Glory",
                                                 "Willow",
                                                 "Lana",
                                                 "Pam",
                                                 "Cheryl",
                                                 "Carol",
                                                 "Malory",
                                                 "Anya",
                                                 "Dawn",
                                                 "Ginger",
                                                 "Chastity",
                                                 "Destiny",
                                                 "Amber",
                                                 "Juilet"
                                             });
        }

        private static void SetupBoyNames()
        {
            _boyNames = ImmutableArray.From(new string[]
                                             {
                                                "Jim",
                                                "Jake",
                                                "Johnny",
                                                "Jose",
                                                "Ian",
                                                "Kevin",
                                                "Nate",
                                                "Brett",
                                                "Dennis",
                                                "Donald",
                                                "Douglas",
                                                "David",
                                                "Vernon",
                                                "Shane",
                                                "Shawn",
                                                "Burton",
                                                "Dean",
                                                "Sam",
                                                "Sterling",
                                                "Cyril",
                                                "Xander",
                                                "Rupert",
                                                "Spike",
                                                "Angel"
                                             });
        }
    }
}
