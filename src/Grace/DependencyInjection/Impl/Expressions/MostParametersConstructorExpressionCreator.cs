using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates a constructor expression when using most parameters
    /// </summary>
    public class MostParametersConstructorExpressionCreator : ConstructorExpressionCreator
    {
        /// <summary>
        /// This method is called when there are multiple constructors
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="configuration"></param>
        /// <param name="request"></param>
        /// <param name="constructors"></param>
        /// <returns></returns>
        protected override ConstructorInfo PickConstructor(IInjectionScope injectionScope, TypeActivationConfiguration configuration,
            IActivationExpressionRequest request, ConstructorInfo[] constructors)
        {
            return constructors.OrderByDescending(c => c.GetParameters().Length).First();
        }
    }
}
