using Grace.DependencyInjection;

namespace Grace.Tests.Classes.Simple
{
    public interface IImportBasicServiceMetadata
    {
        IBasicService BasicService { get; }

        IActivationStrategyMetadata Metadata { get; }
    }

    public class ImportBasicServiceMetadata : IImportBasicServiceMetadata
    {
        public ImportBasicServiceMetadata(Meta<IBasicService> meta)
        {
            BasicService = meta.Value;
            Metadata = meta.Metadata;
        }

        public IBasicService BasicService { get; }

        public IActivationStrategyMetadata Metadata { get; }
    }
}
