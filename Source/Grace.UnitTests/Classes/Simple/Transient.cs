namespace Grace.UnitTests.Classes.Simple
{
	public interface ITransient
	{
		IBasicService BasicService { get; set; }
	}

	public class Transient : ITransient
	{
		public IBasicService BasicService { get; set; }
	}
}