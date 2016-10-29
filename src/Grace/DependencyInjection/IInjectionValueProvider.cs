using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    public interface IInjectionValueProvider
    {
        IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
