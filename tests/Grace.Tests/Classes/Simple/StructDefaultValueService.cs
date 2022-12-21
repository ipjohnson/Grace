using System;

namespace Grace.Tests.Classes.Simple
{
    public class StructDefaultValueService
    {
        public StructDefaultValueService(DateTime dateTime = new DateTime(),
            DateTime dateTimeDefault = default(DateTime),
            TimeSpan timeSpan = new TimeSpan(),
            TimeSpan timeSpanDefault = default(TimeSpan),
            DateTimeOffset dateTimeOffset = new DateTimeOffset(),
            DateTimeOffset dateTimeOffsetDefault = default(DateTimeOffset),
            Guid guid = new Guid(),
            Guid guidDefault = default(Guid),
            CustomStruct customStruct = new CustomStruct(),
            CustomStruct customStructDefault = default(CustomStruct),
            ConsoleColor? color = ConsoleColor.DarkGreen,
            ConsoleColor? colorNull = null,
            int? integer = 12,
            int? integerNull = null)
        {
            Color = color;
            ColorNull = colorNull;
            Integer = integer;
            IntegerNull = integerNull;
        }

        public struct CustomStruct { }

        public ConsoleColor? Color { get; }

        public ConsoleColor? ColorNull { get; }

        public int? Integer { get; }

        public int? IntegerNull { get; }
    }
}
