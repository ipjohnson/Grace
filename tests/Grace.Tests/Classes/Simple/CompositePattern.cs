using System.Collections.Generic;
using System.Linq;

namespace Grace.Tests.Classes.Simple
{
    public interface ICompositePattern
    {
        int Count { get; }
    }

    public class CompositePattern : ICompositePattern
    {
        private readonly IEnumerable<ICompositePattern> _patterns;

        public CompositePattern(IEnumerable<ICompositePattern> patterns)
        {
            _patterns = patterns;
        }

        public int Count => _patterns.Aggregate(0, (i, pattern) => i + pattern.Count);
    }

    public class SimpleCompositePattern1 : ICompositePattern
    {
        public int Count => 1;
    }

    public class SimpleCompositePattern2 : ICompositePattern
    {
        public int Count => 2;
    }
}
