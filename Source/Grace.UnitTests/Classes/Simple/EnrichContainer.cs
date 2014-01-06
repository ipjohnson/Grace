namespace Grace.UnitTests.Classes.Simple
{
	public class EnrichContainer
	{
		public EnrichContainer(object enrichedObject)
		{
			EnrichedObject = enrichedObject;
		}

		public object EnrichedObject { get; private set; }
	}
}