using System;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportDateTimeByName
	{
		public ImportDateTimeByName(DateTime dateTime)
		{
			DateTime = dateTime;
		}

		public DateTime DateTime { get; private set; }
	}
}