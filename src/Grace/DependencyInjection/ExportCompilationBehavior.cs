using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Enumeration for constructor selection method
    /// </summary>
    public static class ConstructorSelectionMethod
    {
        static ConstructorSelectionMethod()
        {
            LeastParameters = new LeastParametersConstructorExpressionCreator();

            MostParameters = new MostParametersConstructorExpressionCreator();

            BestMatch = new BestMatchConstructorExpressionCreator();

            Dynamic = new DynamicConstructorExpressionCreator();
        }

        /// <summary>
        /// Matches the best constructor based on which exports are registered
        /// </summary>
        public static IConstructorExpressionCreator BestMatch { get; }

        /// <summary>
        /// Use the constructor with the most parameters
        /// </summary>
        public static IConstructorExpressionCreator MostParameters { get; }

        /// <summary>
        /// Use the constructor with the least parameters
        /// </summary>
        public static  IConstructorExpressionCreator LeastParameters { get; }

        /// <summary>
        /// Dynamicly pick the cosntructor to use each request, not very fast but allows for support similar to NInject
        /// </summary>
        public static IConstructorExpressionCreator Dynamic { get; }
    }

    /// <summary>
    /// Classes that implement this can be used to create enumerables
    /// </summary>
    public interface IEnumerableCreator
    {
        /// <summary>
        /// Construct enumerable given a scope and an array
        /// </summary>
        /// <typeparam name="T">Type to enumerate</typeparam>
        /// <param name="scope">export locator scope</param>
        /// <param name="array">array to wrap as enumerable</param>
        /// <returns>enumerable</returns>
        IEnumerable<T> CreateEnumerable<T>(IExportLocatorScope scope, T[] array);
    }
    
    /// <summary>
    /// Configure the how expressions are created
    /// </summary>
    public class ExportCompilationBehavior
    {
        private Func<Type, bool> _keyedTypeSelector = DefaultKeyedTypeSelector;

        /// <summary>
        /// Default implementation for selecting types that should be located by key.
        /// Note: string, primitive and datetime are located by key
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool DefaultKeyedTypeSelector(Type arg)
        {
            if (arg.GetTypeInfo().IsAssignableFrom(typeof(Delegate).GetTypeInfo()))
            {
                return false;
            }

            return arg == typeof(string) || arg.GetTypeInfo().IsPrimitive || arg == typeof(DateTime);
        }

        /// <summary>
        /// Max object graph depth, this is what's used to detect a recursive loop
        /// </summary>
        /// <returns></returns>
        public int MaxObjectGraphDepth { get; set; } = 100;

        /// <summary>
        /// Allow IInjectionScope to be injected, false by default because you normally want IExportLocatorScope
        /// </summary>
        public bool AllowInjectionScopeLocation { get; set; } = false;

        /// <summary>
        /// Constructor selection algorithm, best match by default
        /// </summary>
        public IConstructorExpressionCreator ConstructorSelection { get; set; } = ConstructorSelectionMethod.BestMatch;

        /// <summary>
        /// customize IEnumerable&lt;T&gt; creation
        /// </summary>
        public IEnumerableCreator CustomEnumerableCreator { get; set; }

        /// <summary>
        /// Allows you to override the default behavior for what is located by key and what's not 
        /// Note: By default string, primitive and DateTime are true, everything else is false
        /// </summary>
        public Func<Type, bool> KeyedTypeSelector
        {
            get => _keyedTypeSelector;
            set => _keyedTypeSelector = value ?? throw new ArgumentNullException(nameof(KeyedTypeSelector), "value must not be null");
        }

        /// <summary>
        /// By default ExportInstance and ExportFactory must return a value. 
        /// </summary>
        /// <returns></returns>
        public bool AllowInstanceAndFactoryToReturnNull { get; set; } = false;

        /// <summary>
        /// Process ImportAttribute for parameteres
        /// </summary>
        public bool ProcessImportAttributeForParameters { get; set; } = true;
    }
}
