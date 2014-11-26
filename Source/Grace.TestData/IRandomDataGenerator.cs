using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData
{
    public enum StringType
    {
        MostCharacter,
        Alpha,
        AlphaNumeric,
        Numeric,
    }
    public interface IRandomDataGeneratorService
    {
        string NextString(StringType stringType = StringType.MostCharacter, int min = 5, int max = 16);
        
        bool NextBool();

        char NextChar(char min = char.MinValue, char max = char.MaxValue);

        /// <summary>
        /// Next alpha character
        /// </summary>
        /// <returns></returns>
        char NextCharacter();

        byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue);

        short NextInt16(Int16 min = Int16.MinValue, Int16 max = Int16.MaxValue);

        ushort NextUInt16(UInt16 min = UInt16.MinValue, UInt16 max = UInt16.MaxValue);

        int NextInt(int min = int.MinValue, int max = int.MaxValue);

        uint NextUInt32(UInt32 min = UInt32.MinValue, UInt32 max = UInt32.MaxValue);

        long NextInt64(Int64 min = Int64.MinValue, Int64 max = Int64.MaxValue);

        ulong NextUInt64(UInt64 min = UInt64.MinValue, UInt64 max = UInt64.MaxValue);

        double NextDouble(double min = double.MinValue, double max = double.MaxValue);

        decimal NextDecimal(decimal? min = null, decimal? max = null);
    }
}
