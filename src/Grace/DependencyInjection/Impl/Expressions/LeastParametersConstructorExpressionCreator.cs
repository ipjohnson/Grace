using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates constructor expression for least parameters
    /// </summary>
    public class LeastParametersConstructorExpressionCreator : ConstructorExpressionCreator
    {
        /// <summary>
        /// This method is called when there are multiple constructors
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructors"></param>
        protected override ConstructorInfo PickConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration,
            IActivationExpressionRequest request, ConstructorInfo[] constructors)
        {
            #if NET6_0_OR_GREATER
            return constructors.MinBy(c => c.GetParameters().Length);
            #else
            return constructors.OrderBy(c => c.GetParameters().Length).First();
            #endif
        }
    }
}
