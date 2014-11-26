using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Selenium.Data
{
    public interface IRandomDataService
    {
        int Int(int min = int.MinValue, int max = int.MaxValue);

        double Double(double min = double.MinValue, double max = double.MaxValue);

        string AlphaNumericString(int minLength = 5, int maxLength = 16);

        string FormattedString(string format);
    }
}
