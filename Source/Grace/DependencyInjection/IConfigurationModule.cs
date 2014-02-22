using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes that implement this method can be used to configure a registration scope or container
	/// </summary>
	public interface IConfigurationModule
	{
		/// <summary>
		/// Called by the injection scope to add exports to the scope
		/// </summary>
		/// <param name="registrationBlock"></param>
		void Configure([NotNull] IExportRegistrationBlock registrationBlock);
	}
}