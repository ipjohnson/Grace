using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.DataSources
{
    public class StateDataSource : BaseDataSource<string>
    {
        internal static readonly ImmutableArray<Tuple<string, string>> States;

        static StateDataSource()
        {
            States = ImmutableArray.From(new []
                                          {
                                              new Tuple<string, string>("Alabama","AL"),
                                              new Tuple<string, string>("ALASKA","AK"),
                                              new Tuple<string, string>("ARIZONA","AZ"),
                                              new Tuple<string, string>("ARKANSAS","AR"),
                                              new Tuple<string, string>("CALIFORNIA","CA"),
                                              new Tuple<string, string>("COLORADO","CO"),
                                              new Tuple<string, string>("CONNECTICUT","CT"),
                                              new Tuple<string, string>("DELAWARE","DE"),
                                              new Tuple<string, string>("FLORIDA","FL"),
                                              new Tuple<string, string>("GEORGIA","GA"),
                                              new Tuple<string, string>("HAWAII","HI"),
                                              new Tuple<string, string>("IDAHO","ID"),
                                              new Tuple<string, string>("ILLINOIS","IL"),
                                              new Tuple<string, string>("INDIANA","IN"),
                                              new Tuple<string, string>("IOWA","IA"),
                                              new Tuple<string, string>("KANSAS","KS"),
                                              new Tuple<string, string>("KENTUCKY","KY"),
                                              new Tuple<string, string>("LOUISIANA","LA"),
                                              new Tuple<string, string>("MAINE","ME"),
                                              new Tuple<string, string>("MARYLAND","MD"),
                                              new Tuple<string, string>("MASSACHUSETTS","MA"),
                                              new Tuple<string, string>("MICHIGAN","MI"),
                                              new Tuple<string, string>("MINNESOTA","MN"),
                                              new Tuple<string, string>("MISSISSIPPI","MS"),
                                              new Tuple<string, string>("MISSOURI","MO"),
                                              new Tuple<string, string>("MONTANA","MT"),
                                              new Tuple<string, string>("NEBRASKA","NE"),
                                              new Tuple<string, string>("NEVADA","NV"),
                                              new Tuple<string, string>("NEW HAMPSHIRE","NH"),
                                              new Tuple<string, string>("NEW JERSEY","NJ"),
                                              new Tuple<string, string>("NEW MEXICO","NM"),
                                              new Tuple<string, string>("NEW YORK","NY"),
                                              new Tuple<string, string>("NORTH CAROLINA","NC"),
                                              new Tuple<string, string>("NORTH DAKOTA","ND"),
                                              new Tuple<string, string>("OHIO","OH"),
                                              new Tuple<string, string>("OKLAHOMA","OK"),
                                              new Tuple<string, string>("OREGON","OR"),
                                              new Tuple<string, string>("PENNSYLVANIA","PA"),
                                              new Tuple<string, string>("RHODE ISLAND","RI"),
                                              new Tuple<string, string>("SOUTH CAROLINA","SC"),
                                              new Tuple<string, string>("SOUTH DAKOTA","SD"),
                                              new Tuple<string, string>("TENNESSEE","TN"),
                                              new Tuple<string, string>("TEXAS","TX"),
                                              new Tuple<string, string>("UTAH","UT"),
                                              new Tuple<string, string>("VERMONT","VT"),
                                              new Tuple<string, string>("VIRGINIA","VA"),
                                              new Tuple<string, string>("WASHINGTON","WA"),
                                              new Tuple<string, string>("WEST VIRGINIA","WV"),
                                              new Tuple<string, string>("WISCONSIN","WI"),
                                              new Tuple<string, string>("WYOMING","WY")
                                          });
        }
        
        private readonly IRandomDataGeneratorService _randomData;

        public StateDataSource(IRandomDataGeneratorService randomData)
        {
            _randomData = randomData;
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return States[_randomData.NextInt(0, States.Length)];
        }
    }
}
