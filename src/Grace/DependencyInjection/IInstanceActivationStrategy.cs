using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface for instance and factory strategies
    /// </summary>
    public interface IInstanceActivationStrategy : IConfigurableActivationStrategy
    {
        /// <summary>
        /// Allow for null return of instance
        /// </summary>
        bool? AllowNullReturn { get; set; }
    }
}
