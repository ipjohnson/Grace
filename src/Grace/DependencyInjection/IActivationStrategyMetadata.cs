using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    public interface IActivationStrategyMetadata : IReadOnlyDictionary<object,object>
    {
        Type ActivationType { get; }

        IEnumerable<Type> ExportAs { get; }

        IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed { get; }

        bool MetadataMatches(object key, object value);
    }
}
