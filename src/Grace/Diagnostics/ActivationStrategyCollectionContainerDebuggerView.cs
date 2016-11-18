using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class used for visual studio debugging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActivationStrategyCollectionContainerDebuggerView<T> where T : IActivationStrategy
    {
        private readonly IActivationStrategyCollectionContainer<T> _container;
        private readonly List<StrategyListDebuggerView<T>> _strategiesByType;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="container"></param>
        public ActivationStrategyCollectionContainerDebuggerView(IActivationStrategyCollectionContainer<T> container)
        {
            _container = container;
            _strategiesByType = new List<StrategyListDebuggerView<T>>();

            foreach (var type in container.GetActivationTypes())
            {
                var list = new List<T>();

                var collection = container.GetActivationStrategyCollection(type);

                foreach (var strategy in collection.GetStrategies())
                {
                    list.Add(strategy);
                }

                foreach (var keyedStrategy in collection.GetKeyedStrategies())
                {
                    list.Add(keyedStrategy.Value);
                }

                _strategiesByType.Add(new StrategyListDebuggerView<T>(type, list));
            }

            _strategiesByType.Sort(
                (x, y) => string.Compare(x.Type.FullName, y.Type.FullName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// All strategies
        /// </summary>
        public IEnumerable<T> Strategies => _container.GetAllStrategies();

        /// <summary>
        /// Strategies by type
        /// </summary>
        public IEnumerable<StrategyListDebuggerView<T>> StrategiesByType => _strategiesByType;

        /// <summary>
        /// Strategies debugger view
        /// </summary>
        /// <typeparam name="TStrategy"></typeparam>
        [DebuggerDisplay("{DebuggerDisplayValue,nq}", Name = "{DebuggerDisplayName,nq}")]
        public class StrategyListDebuggerView<TStrategy>
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly List<TStrategy> _strategies;

            /// <summary>
            /// Default Constructor
            /// </summary>
            /// <param name="type"></param>
            /// <param name="strategies"></param>
            public StrategyListDebuggerView(Type type, List<TStrategy> strategies)
            {
                _strategies = strategies;
                Type = type;
            }

            /// <summary>
            /// Type of strategies
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Type Type { get; }

            /// <summary>
            /// Items
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IEnumerable<TStrategy> Items => _strategies;

            /// <summary>
            /// Debugger display value
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplayValue => $"Count: {_strategies.Count}";

            /// <summary>
            /// Debugger display name
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplayName => Type.FullName;
        }
    }
}