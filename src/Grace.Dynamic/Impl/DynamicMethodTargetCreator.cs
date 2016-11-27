using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Exceptions;

namespace Grace.Dynamic.Impl
{
    public interface IDynamicMethodTargetCreator
    {
        object CreateMethodTarget(DynamicMethodGenerationRequest request);
    }

    public class DynamicMethodTargetCreator : IDynamicMethodTargetCreator
    {
        public object CreateMethodTarget(DynamicMethodGenerationRequest request)
        {
            switch (request.Constants.Count)
            {
                case 0:
                    return new DynamicMethodTarget();
                case 1:
                case 2:
                case 3:
                    return CreateConstantTarget(request);
            }

            return null;
        }

        private object CreateConstantTarget(DynamicMethodGenerationRequest request)
        {
            Type openType = null;

            switch (request.Constants.Count)
            {
                case 1:
                    openType = typeof(DynamicMethodTarget<>);
                    break;

                case 2:
                    openType = typeof(DynamicMethodTarget<,>);
                    break;

                case 3:
                    openType = typeof(DynamicMethodTarget<,,>);
                    break;

                default:
                    return null;
            }

            var closedType = openType.MakeGenericType(request.Constants.Select(c => c.GetType()).ToArray());

            return Activator.CreateInstance(closedType, request.Constants.ToArray());
        }
    }
}
