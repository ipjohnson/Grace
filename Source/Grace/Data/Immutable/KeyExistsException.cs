using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data.Immutable
{
    /// <summary>
    /// Key already exists in ImmutableHashTree
    /// </summary>
    public class KeyExistsException<TKey> : Exception
    {
        /// <summary>
        /// Default constructor takes key
        /// </summary>
        public KeyExistsException() 
        {
            
        }
    }
}
