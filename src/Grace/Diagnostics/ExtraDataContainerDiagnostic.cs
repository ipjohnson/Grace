using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Class used for diagnostic info in the debugger
    /// </summary>
    public class ExtraDataContainerDiagnostic
    {
        private IExtraDataContainer _extraDataContainer;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="extraDataContainer"></param>
        public ExtraDataContainerDiagnostic(IExtraDataContainer extraDataContainer)
        {
            _extraDataContainer = extraDataContainer;
            
        }
    }
}
