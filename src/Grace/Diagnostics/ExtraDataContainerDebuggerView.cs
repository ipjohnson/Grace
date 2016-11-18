using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Debugger view for extra data
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}")]
    public class ExtraDataContainerDebuggerView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IExtraDataContainer _extraDataContainer;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="extraDataContainer"></param>
        public ExtraDataContainerDebuggerView(IExtraDataContainer extraDataContainer)
        {
            _extraDataContainer = extraDataContainer;
        }

        /// <summary>
        /// Items in extra data
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IEnumerable<KeyValuePairDebuggerView<object, object>> Items
        {
            get
            {
                var list =
                    _extraDataContainer.KeyValuePairs.Select(kvp => new KeyValuePairDebuggerView<object, object>(kvp.Key, kvp.Value)).ToList();

                list.Sort((x, y) => string.Compare(x?.ToString() ?? "", y?.ToString() ?? "", StringComparison.CurrentCultureIgnoreCase));

                return list;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplayValue => "Count: " + _extraDataContainer.KeyValuePairs.Count();
    }
}
