using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.Tests.Classes.Simple
{
    public class DefaultStructParameterService
    {
        public DefaultStructParameterService(DateTime time = new DateTime(),DateTime anotherDate = default(DateTime), CustomStruct customStruct = new CustomStruct(), CustomStruct another = default(CustomStruct))
        {
            Time = time;
            CustomStruct = customStruct;
        }

        public DateTime Time { get; }

        public CustomStruct CustomStruct { get; }
    }

    public struct CustomStruct
    {

    }
}
