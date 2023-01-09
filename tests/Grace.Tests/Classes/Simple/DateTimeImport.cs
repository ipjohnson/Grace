using System;

namespace Grace.Tests.Classes.Simple
{
    public class DateTimeImport
    {
        public DateTimeImport(DateTime currentTime)
        {
            CurrentTime = currentTime;
        }

        public DateTime CurrentTime { get; }
    }

    public class NowDateTimeImport
    {
        public NowDateTimeImport(DateTime now)
        {
            CurrentTime = now;
        }

        public DateTime CurrentTime { get; }
    }
}
