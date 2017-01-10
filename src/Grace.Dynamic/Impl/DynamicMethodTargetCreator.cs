using System;
using System.Linq;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// interface for creating an object to bind dynamic method to
    /// </summary>
    public interface IDynamicMethodTargetCreator
    {
        /// <summary>
        /// Create method target based on request
        /// </summary>
        /// <param name="request">dynamic method request</param>
        /// <returns></returns>
        object CreateMethodTarget(DynamicMethodGenerationRequest request);
    }

    /// <summary>
    /// Class for creating dynamic method target
    /// </summary>
    public class DynamicMethodTargetCreator : IDynamicMethodTargetCreator
    {
        /// <summary>
        /// Create method target based on request
        /// </summary>
        /// <param name="request">dynamic method request</param>
        /// <returns></returns>
        public object CreateMethodTarget(DynamicMethodGenerationRequest request)
        {
            return request.Constants.Count == 0 ? new DynamicMethodTarget() : CreateConstantTarget(request);
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
                case 4:
                    openType = typeof(DynamicMethodTarget<,,,>);
                    break;

                case 5:
                    openType = typeof(DynamicMethodTarget<,,,,>);
                    break;

                case 6:
                    openType = typeof(DynamicMethodTarget<,,,,,>);
                    break;

                case 7:
                    openType = typeof(DynamicMethodTarget<,,,,,,>);
                    break;

                case 8:
                    openType = typeof(DynamicMethodTarget<,,,,,,,>);
                    break;

                default:
                    return CreateArrayTarget(request);
            }

            var closedType = openType.MakeGenericType(request.Constants.Select(c => c.GetType()).ToArray());

            return Activator.CreateInstance(closedType, request.Constants.ToArray());
        }

        private object CreateArrayTarget(DynamicMethodGenerationRequest request)
        {
            request.IsArrayTarget = true;

            return new ArrayDynamicMethodTarget(request.Constants.ToArray());
        }
    }
}
