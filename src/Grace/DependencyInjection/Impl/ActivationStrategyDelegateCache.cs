using System;
using System.Collections.Generic;
using System.Text;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class ActivationStrategyDelegateCache
    {
        private readonly int _arrayLengthMinusOne;

        private readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] _activationDelegates;
        private ImmutableHashTree<TypeKeyClass, ActivationStrategyDelegate>[] _keyedDelegates;

        public ActivationStrategyDelegateCache(int cacheArray)
        {
            _arrayLengthMinusOne = cacheArray - 1;
            _activationDelegates = new ImmutableHashTree<Type, ActivationStrategyDelegate>[cacheArray];

            for (var i = 0; i <= _arrayLengthMinusOne; i++)
            {
                _activationDelegates[i] = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(Type type)
        {
            var hashCode = type.GetHashCode();

            var currentNode = _activationDelegates[hashCode & _arrayLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value;
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }
            
            return ReferenceEquals(currentNode.Key, type)
                ? currentNode.Value
                : currentNode.GetConflictedValue(type, currentNode, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActivationStrategyDelegate GetKeyedActivationStrategyDelegate(Type type, object key)
        {
            if (_keyedDelegates == null)
            {
                return null;
            }

            var typeKey = new TypeKeyClass(type, key);

            var hashCode = typeKey.GetHashCode();

            var currentNode = _keyedDelegates[hashCode & _arrayLengthMinusOne];
            
            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return typeKey.Equals(currentNode.Key)
                ? currentNode.Value
                : currentNode.GetConflictedValue(typeKey, currentNode, null);
        }

        /// <summary>
        /// Add activation strategy delegate to cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="activationStrategyDelegate"></param>
        public ActivationStrategyDelegate AddActivationStrategyDelegate(Type type, ActivationStrategyDelegate activationStrategyDelegate)
        {
            var hashCode = type.GetHashCode();

            return ImmutableHashTree.ThreadSafeAdd(ref _activationDelegates[hashCode & _arrayLengthMinusOne], type,
                activationStrategyDelegate);
        }

        /// <summary>
        /// Add keyed activation strategy delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="activationStrategyDelegate"></param>
        /// <returns></returns>
        public ActivationStrategyDelegate AddKeyedActivationStrategyDelegate(Type type, object key,
            ActivationStrategyDelegate activationStrategyDelegate)
        {
            if (_keyedDelegates == null)
            {
                lock (this)
                {
                    if (_keyedDelegates == null)
                    {
                        _keyedDelegates = new ImmutableHashTree<TypeKeyClass, ActivationStrategyDelegate>[_arrayLengthMinusOne + 1];

                        for (int i = 0; i <= _arrayLengthMinusOne; i++)
                        {
                            _keyedDelegates[i] = ImmutableHashTree<TypeKeyClass, ActivationStrategyDelegate>.Empty;
                        }
                    }
                }
            }

            var keyed = new TypeKeyClass(type, key);

            var hashCode = keyed.GetHashCode();

            return ImmutableHashTree.ThreadSafeAdd(ref _keyedDelegates[hashCode & _arrayLengthMinusOne], keyed,
                activationStrategyDelegate);
        }

        #region TypeKeyClass

        public class TypeKeyClass
        {
            public TypeKeyClass(Type type, object key)
            {
                Type = type;
                Key = key;
            }

            public Type Type { get; }

            public object Key { get; }

            /// <summary>Determines whether the specified object is equal to the current object.</summary>
            /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
            /// <param name="obj">The object to compare with the current object. </param>
            public override bool Equals(object obj)
            {
                var compare = obj as TypeKeyClass;

                if (compare == null)
                {
                    return false;
                }

                return ReferenceEquals(compare.Type, Type) &&
                    (ReferenceEquals(Key, compare.Key) || Key.Equals(compare.Key));
            }

            /// <summary>
            /// Override gethashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return Type.GetHashCode() + Key.GetHashCode();
                }
            }

        }
        #endregion
    }
}
