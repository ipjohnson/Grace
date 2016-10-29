using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection
{

    public enum ConstructorSelectionMethod
    {
        /// <summary>
        /// Matches the best constructor based on which exports are registered
        /// </summary>
        BestMatch,

        /// <summary>
        /// Use the constructor with the most parameters
        /// </summary>
        MostParameters,

        /// <summary>
        /// Use the constructor with the least parameters
        /// </summary>
        LeastParameters,

        /// <summary>
        /// Not implemented but avaliable for extension purposes
        /// </summary>
        Other
    }

    public interface IEnumerableCreator
    {
        IEnumerable<T> CreateEnumerable<T>(IExportLocatorScope scope, T[] array);
    }

    public interface IExportCompilationBehaviorValues
    {
        ConstructorSelectionMethod ConstructorSelection();

        IEnumerableCreator CustomEnumerableCreator();

        Func<Type, bool> KeyedTypeSelector();

        /// <summary>
        /// By default ExportInstance and ExportFactory must return a value. 
        /// </summary>
        /// <returns></returns>
        bool AllowInstanceAndFactoryToReturnNull();

        int MaxObjectGraphDepth();

        bool AllowInjectionScopeLocation { get; set; }
    }

    public class ExportCompilationBehavior : IExportCompilationBehaviorValues
    {
        private ConstructorSelectionMethod _constructorSelection = DependencyInjection.ConstructorSelectionMethod.BestMatch;
        private IEnumerableCreator _enumerableCreator;
        private Func<Type, bool> _keyedTypeSelector = DefaultKeyedTypeSelector;
        private bool _allowInstanceAndFactoryToReturnNull;
        private int _depth = 100;

        public void ConstructorSelection(ConstructorSelectionMethod selection)
        {
            _constructorSelection = selection;
        }

        ConstructorSelectionMethod IExportCompilationBehaviorValues.ConstructorSelection()
        {
            return _constructorSelection;
        }

        /// <summary>
        /// Set the type that will be used for IEnumerable, by default this is an array
        /// </summary>
        public void CustomEnumerableCreator(IEnumerableCreator creator)
        {
            _enumerableCreator = creator;
        }

        IEnumerableCreator IExportCompilationBehaviorValues.CustomEnumerableCreator()
        {
            return _enumerableCreator;
        }

        public void KeyedTypeSelector(Func<Type, bool> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            _keyedTypeSelector = selector;
        }

        public Func<Type, bool> KeyedTypeSelector()
        {
            return _keyedTypeSelector;
        }

        public static bool DefaultKeyedTypeSelector(Type arg)
        {
            if (arg.GetTypeInfo().IsAssignableFrom(typeof(Delegate).GetTypeInfo()))
            {
                return false;
            }

            return arg == typeof(string) || arg.GetTypeInfo().IsPrimitive || arg == typeof(DateTime);
        }

        public void AllowInstanceAndFactoryToReturnNull(bool value)
        {
            _allowInstanceAndFactoryToReturnNull = value;
        }

        public bool AllowInstanceAndFactoryToReturnNull()
        {
            return _allowInstanceAndFactoryToReturnNull;
        }

        public void MaxObjectGraphDepth(int depth)
        {
            _depth = depth;
        }

        public int MaxObjectGraphDepth()
        {
            return _depth;
        }

        public bool AllowInjectionScopeLocation { get; set; } = false;
    }
}
