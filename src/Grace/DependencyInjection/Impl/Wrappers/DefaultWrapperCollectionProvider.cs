using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Impl.KnownTypeStrategies;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Provides a collection of default wrappers
    /// </summary>
    public class DefaultWrapperCollectionProvider : IDefaultWrapperCollectionProvider
    {
        /// <summary>
        /// Provide collection of wrappers
        /// </summary>
        /// <param name="scope">scope</param>
        /// <returns>wrapper collection container</returns>
        public IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> ProvideCollection(IInjectionScope scope)
        {
            var collection = scope.ScopeConfiguration.Implementation
                    .Locate<IActivationStrategyCollectionContainer<ICompiledWrapperStrategy>>();

            var metadataProvider = scope.ScopeConfiguration.Implementation.Locate<IStrongMetadataInstanceProvider>();
            var wrapperExpressionCreator = scope.ScopeConfiguration.Implementation.Locate<IWrapperExpressionCreator>();

            collection.AddStrategy(new LazyWrapperStrategy(scope));
            collection.AddStrategy(new LazyMetadataWrapperStrategy(scope, metadataProvider,wrapperExpressionCreator));
            collection.AddStrategy(new OwnedWrapperStrategy(scope));
            collection.AddStrategy(new MetaWrapperStrategy(scope));
            collection.AddStrategy(new StronglyTypedMetadataWrapperStrategy(scope, metadataProvider));
            
            //collection.AddStrategy(new GenericCompiledWrapperStrategy(typeof(Scoped<>), scope, scope.StrategyCompiler.DefaultStrategyExpressionBuilder) { ExternallyOwned = false });

            collection.AddStrategy(new ScopedActivationStrategy(scope));

            collection.AddStrategy(new FuncWrapperStrategy(scope));
            collection.AddStrategy(new FuncOneArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncTwoArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncThreeArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncFourArgWrapperStrategy(scope));
            collection.AddStrategy(new FuncFiveArgWrapperStrategy(scope));

            collection.AddStrategy(new TypedActivationStrategyDelegateWrapperStrategy(scope));

            return collection;
        }
    }
}
