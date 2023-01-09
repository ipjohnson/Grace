using System.Collections.Generic;

namespace Grace.Tests.Classes.Simple
{
    public interface IImportEnumerableService
    {
        IEnumerable<IMultipleService> Enumerable { get; }
    }

    public class ImportEnumerableService : IImportEnumerableService
    {
        public ImportEnumerableService(IEnumerable<IMultipleService> enumerable)
        {
            Enumerable = enumerable;
        }

        public IEnumerable<IMultipleService> Enumerable { get; }
    }
}
