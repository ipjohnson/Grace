namespace Grace.DependencyInjection.Lifestyle
{
    public interface ICompiledLifestyle
    {
        bool RootRequest { get; }

        ICompiledLifestyle Clone();

        IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest requst, IActivationExpressionResult activationExpression);
    }
}
