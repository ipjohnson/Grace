using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Selenium.Data
{
    interface IAddressRandomeDataService
    {
        object AddressObject();

        string Country();

        string State();

        string PostalCode();

        string Address1();

        string Address2();
    }
}
