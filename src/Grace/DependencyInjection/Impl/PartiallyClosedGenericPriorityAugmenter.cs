using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Increases the priority of partially closed generics
	/// </summary>
	public class PartiallyClosedGenericPriorityAugmenter : IActivationStrategyInspector
	{
	    /// <summary>
	    /// Inspect the activation strategy
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="strategy"></param>
	    public void Inspect<T>(T strategy) where T : class, IActivationStrategy
	    {
	        var configurableStrategy = strategy as IConfigurableActivationStrategy;

	        if (configurableStrategy != null &&
	            configurableStrategy.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
	        {
                var maxClosed = 0;
                var openGenericParams = configurableStrategy.ActivationType.GetTypeInfo().GenericTypeParameters.Length;

	            foreach (var exportType in configurableStrategy.ExportAs)
	            {
                    int exportTypeParamCount = exportType.GetTypeInfo().GenericTypeParameters.Length;
                    int numClosed = exportTypeParamCount - openGenericParams;

                    if (numClosed > maxClosed)
                    {
                        maxClosed = numClosed;
                    }
                }

	            configurableStrategy.Priority = maxClosed;
	        }
        }
	}
}
