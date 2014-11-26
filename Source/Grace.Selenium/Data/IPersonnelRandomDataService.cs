using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Selenium.Data
{
    public interface IPersonnelRandomDataService
    {
        object PersonObject(string emailDomain = null, string phonePrefix = null);

        string EmailAddress(string domain = null);

        string Username();

        string Firstname();

        string Password();

        string Middlename();

        DateTime DateOfBirth();

        string GovernmentUniqueId();

        string PhoneNumber(string prefix = null);
    }
}
