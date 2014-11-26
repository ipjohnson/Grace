using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.TestData.Impl
{
    public class RandomDataGeneratorService : IRandomDataGeneratorService
    {
        private readonly Random _random = new Random();
        private ImmutableArray<char> _allCharacters;
        private ImmutableArray<char> _numericCharacters;
        private ImmutableArray<char> _alphaNumericCharacters;
        private ImmutableArray<char> _alphaCharacters;

        public RandomDataGeneratorService()
        {
            SetupAlphaCharacters();

            SetupNumericCharacters();

            SetupAlphaNumericCharacters();

            SetupAllCharacters();
        }
        
        public ushort NextUInt16(ushort min = UInt16.MinValue, ushort max = UInt16.MaxValue)
        {
            throw new NotImplementedException();
        }

        public int NextInt(int min = Int32.MinValue, int max = Int32.MaxValue)
        {
            return _random.Next(min, max);
        }

        public uint NextUInt32(uint min = UInt32.MinValue, uint max = UInt32.MaxValue)
        {
            return (uint)((_random.NextDouble() * (max - min)) + max);
        }

        public long NextInt64(long min = Int64.MinValue, long max = Int64.MaxValue)
        {
            return (long)((_random.NextDouble() * (max - min)) + max);
        }

        public ulong NextUInt64(ulong min = UInt64.MinValue, ulong max = UInt64.MaxValue)
        {
            return (ulong)((_random.NextDouble() * (max - min)) + max);
        }

        public decimal NextDecimal(decimal? min = null, decimal? max = null)
        {
            if (!min.HasValue)
            {
                min = decimal.MinValue;
            }

            if (!max.HasValue)
            {
                max = decimal.MaxValue;
            }

            return ((decimal)_random.NextDouble() * (max.Value - min.Value)) + min.Value;
        }

        public string NextString(StringType stringType = StringType.MostCharacter, int min = 5, int max = 16)
        {
            switch (stringType)
            {
                case StringType.Alpha:
                    return BuildString(_alphaCharacters, min, max);

                case StringType.AlphaNumeric:
                    return BuildString(_alphaNumericCharacters, min, max);

                case StringType.Numeric:
                    return BuildString(_numericCharacters, min, max);

                default:
                    return BuildString(_allCharacters, min, max);
            }
        }

        public double NextDouble(double min = Double.MinValue, double max = Double.MaxValue)
        {
            if (min == double.MinValue)
            {
                min = double.MinValue + .1;
            }

            if (max == double.MaxValue)
            {
                max = double.MaxValue - .1;
            }

            return _random.NextDouble() * (max - min) + min;
        }

        public bool NextBool()
        {
            return _random.Next() % 2 == 0;
        }

        public char NextChar(char min = Char.MinValue, char max = Char.MaxValue)
        {
            return Convert.ToChar((int)((_random.NextDouble() * (max - min)) + max));
        }

        public char NextCharacter()
        {
            return _alphaCharacters[NextInt(0, _alphaCharacters.Length)];
        }

        public byte NextByte(byte min = Byte.MinValue, byte max = Byte.MaxValue)
        {
            return Convert.ToByte((int)((_random.NextDouble() * (max - min)) + max));
        }

        public short NextInt16(short min = Int16.MinValue, short max = Int16.MaxValue)
        {
            return Convert.ToInt16((int)((_random.NextDouble() * (max - min)) + max));
        }

        private string BuildString(ImmutableArray<char> characters, int min, int max)
        {
            StringBuilder builder = new StringBuilder();
            int length = _random.Next(min, max);
            int totalCharacters = characters.Length;

            for (int i = 0; i < length; i++)
            {
                builder.Append(characters[_random.Next(0, totalCharacters + 1)]);
            }

            return builder.ToString();
        }

        private void SetupAlphaCharacters()
        {
            List<char> list = new List<char>();

            for (char i = 'a'; i <= 'z'; i++)
            {
                list.Add(i);
            }

            for (char i = 'A'; i <= 'Z'; i++)
            {
                list.Add(i);
            }

            _alphaCharacters = ImmutableArray.From(list);
        }

        private void SetupNumericCharacters()
        {
            List<char> list = new List<char>();

            for (char i = '0'; i <= '9'; i++)
            {
                list.Add(i);
            }

            _numericCharacters = ImmutableArray.From(list);
        }

        private void SetupAlphaNumericCharacters()
        {
            List<char> list = new List<char>(_alphaCharacters);

            list.AddRange(_numericCharacters);

            _alphaNumericCharacters = ImmutableArray.From(list);
        }

        private void SetupAllCharacters()
        {
            List<char> list = new List<char>
                              {
                                  '!',
                                  '@',
                                  '#',
                                  '$',
                                  '%',
                                  '^',
                                  '&',
                                  '*',
                                  '?',
                                  ';',
                                  '(',
                                  ')',
                                  '\\',
                                  '[',
                                  ']',
                                  ',',
                                  '.',
                                  '/',
                                  '_',
                                  '-',
                                  '+',
                                  '=',
                                  '~',
                                  '`',
                                  ':',
                                  '<',
                                  '>'
                              };

            list.AddRange(_alphaNumericCharacters);

            _allCharacters = ImmutableArray.From(list);
        }
    }
}
