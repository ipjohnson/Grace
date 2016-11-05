using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Process attributes on activation strategy
    /// </summary>
    public interface IActivationStrategyAttributeProcessor
    {
        /// <summary>
        /// Process attribute on strategy
        /// </summary>
        /// <param name="strategy">activation strategy</param>
        void ProcessAttributeForConfigurableActivationStrategy(IConfigurableActivationStrategy strategy);
    }

    /// <summary>
    /// Process attributes on strategy
    /// </summary>
    public class ActivationStrategyAttributeProcessor : IActivationStrategyAttributeProcessor
    {
        /// <summary>
        /// Process attribute on strategy
        /// </summary>
        /// <param name="strategy">activation strategy</param>
        public void ProcessAttributeForConfigurableActivationStrategy(IConfigurableActivationStrategy strategy)
        {
            ProcessClassAttributes(strategy);

            ProcessFields(strategy);

            ProcessProperties(strategy);

            ProcessMethods(strategy);
        }

        private void ProcessMethods(IConfigurableActivationStrategy strategy)
        {
            
        }

        private void ProcessProperties(IConfigurableActivationStrategy strategy)
        {

        }

        private void ProcessFields(IConfigurableActivationStrategy strategy)
        {

        }

        private void ProcessClassAttributes(IConfigurableActivationStrategy strategy)
        {
            
        }
    }
}
