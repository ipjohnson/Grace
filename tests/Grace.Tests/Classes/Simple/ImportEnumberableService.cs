using System.Collections.Generic;

namespace Grace.Tests.Classes.Simple
{
    public interface IImportEnumberableService
    {
        IEnumerable<IMultipleService> Enumerable { get; }
    }

    public class ImportEnumberableService : IImportEnumberableService
    {
        public ImportEnumberableService(IEnumerable<IMultipleService> enumerable)
        {
            Enumerable = enumerable;
        }

        public IEnumerable<IMultipleService> Enumerable { get; }
    }
}
