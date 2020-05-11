using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Grace.Data.Immutable;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Cache of activation delegates
    /// </summary>
    public class ActivationStrategyDelegateCache
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate>[] _activationDelegates;
        private const int _activationDelegatesLengthMinusOne = 63;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ActivationStrategyDelegateCache()
        {
            _activationDelegates = new ImmutableHashTree<Type, ActivationStrategyDelegate>[_activationDelegatesLengthMinusOne + 1];

            for (var i = 0; i <= _activationDelegatesLengthMinusOne; i++)
            {
                _activationDelegates[i] = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
            }
        }

        /// <summary>
        /// Find and execute activation strategy delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public object ExecuteActivationStrategyDelegate(Type type, IExportLocatorScope scope)
        {
            var hashCode = RuntimeHelpers.GetHashCode(type);
            var currentNode = _activationDelegates[hashCode & _activationDelegatesLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value(scope, scope, null);
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return ReferenceEquals(currentNode.Key, type) ?
                currentNode.Value(scope, scope, null) :
                FallbackExecution(currentNode, type, scope, false, null);
        }

        /// <summary>
        /// Find and execute activation strategy delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public object ExecuteActivationStrategyDelegateAllowNull(Type type, IExportLocatorScope scope)
        {
            var hashCode = RuntimeHelpers.GetHashCode(type);
            var currentNode = _activationDelegates[hashCode & _activationDelegatesLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value(scope, scope, null);
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return ReferenceEquals(currentNode.Key, type) ?
                currentNode.Value(scope, scope, null) :
                FallbackExecution(currentNode, type, scope, true, null);
        }

        /// <summary>
        /// Find and execute activation strategy delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <param name="allowNull"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object ExecuteActivationStrategyDelegateWithContext(Type type, IExportLocatorScope scope, bool allowNull, IInjectionContext context)
        {
            var hashCode = RuntimeHelpers.GetHashCode(type);
            var currentNode = _activationDelegates[hashCode & _activationDelegatesLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value(scope, scope, context);
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return ReferenceEquals(currentNode.Key, type) ?
                currentNode.Value(scope, scope, context) :
                FallbackExecution(currentNode, type, scope, allowNull, context);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private object FallbackExecution(ImmutableHashTree<Type, ActivationStrategyDelegate> currentNode, Type type,
            IExportLocatorScope scope, bool allowNull, IInjectionContext context)
        {
            if (currentNode.Height != 0)
            {
                foreach (var kvp in currentNode.Conflicts)
                {
                    if (ReferenceEquals(kvp.Key, type))
                    {
                        return kvp.Value(scope, scope, context);
                    }
                }
            }

            var injectionScope = scope.GetInjectionScope();

            return injectionScope.LocateFromChildScope(scope, scope, type, context, null, null, allowNull, false);
        }


        /// <summary>
        /// Add delegate to cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="activationStrategyDelegate"></param>
        public void AddDelegate(Type type, ActivationStrategyDelegate activationStrategyDelegate)
        {
            var hashCode = type.GetHashCode();

            ImmutableHashTree.ThreadSafeAdd(ref _activationDelegates[hashCode & _activationDelegatesLengthMinusOne],
                type,
                activationStrategyDelegate);
        }

    }
}
