namespace Grace.DependencyInjection
{
    public interface IInjectionValueProvider
    {
        IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
