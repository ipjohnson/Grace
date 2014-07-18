using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data.Immutable
{
    /// <summary>
    /// Key value pair where using fields instead of properties for maximum performancec
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KV<TKey, TValue>
    {
        /// <summary>
        /// Key portion of key value pair
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// Value portion of key value pair
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// Default constructor taking a key and value
        /// </summary>
        /// <param name="key">key used in KVP</param>
        /// <param name="value">value used in KVP</param>
        public KV(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Immutable HashTree implementation http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ImmutableHashTree<TKey, TValue>
    {
        /// <summary>
        /// Empty hashtree, used as the starting point 
        /// </summary>
        public static readonly ImmutableHashTree<TKey, TValue> Empty = new ImmutableHashTree<TKey, TValue>();

        /// <summary>
        /// Hash value for this node
        /// </summary>
        public readonly int Hash;

        /// <summary>
        /// Height of hashtree node
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Key value for this hash node
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// Value for this hash node
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// Keys with the same hashcode
        /// </summary>
        public readonly ImmutableList<KV<TKey, TValue>> Conflicts;

        /// <summary>
        /// Left node of the hash tree
        /// </summary>
        public readonly ImmutableHashTree<TKey, TValue> Left;

        /// <summary>
        /// Right node of the hash tree
        /// </summary>
        public readonly ImmutableHashTree<TKey, TValue> Right;

        /// <summary>
        /// Update delegate defines behavior when key already exists
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public delegate TValue UpdateDelegate(TValue currentValue, TValue newValue);


        /// <summary>
        /// Adds a new entry to the hashtree
        /// </summary>
        /// <param name="key">key to add</param>
        /// <param name="value">value to add</param>
        /// <param name="updateDelegate">update delegate, by default will throw key already exits exception</param>
        /// <returns></returns>
        public ImmutableHashTree<TKey, TValue> Add(TKey key, TValue value, UpdateDelegate updateDelegate = null)
        {
            if (updateDelegate == null)
            {
                updateDelegate = KeyAlreadyExists;
            }

            return InternalAdd(key.GetHashCode(), key, value, updateDelegate);
        }

        /// <summary>
        /// Try get value from hashtree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            ImmutableHashTree<TKey, TValue> currenNode = this;
            int keyHash = key.GetHashCode();

            while (currenNode.Hash != keyHash && currenNode.Height != 0)
            {
                currenNode = keyHash < currenNode.Hash ? currenNode.Left : currenNode.Right;
            }

            if (currenNode.Height != 0)
            {
                if (key.Equals(currenNode.Key))
                {
                    value = currenNode.Value;

                    return true;
                }

                KV<TKey, TValue> kvp = currenNode.Conflicts.FirstOrDefault(x => key.Equals(x.Key));

                if (kvp != null)
                {
                    value = kvp.Value;

                    return true;
                }
            }

            value = default(TValue);

            return false;
        }

        private ImmutableHashTree()
        {
            Conflicts = ImmutableList<KV<TKey, TValue>>.Empty;
        }

        private ImmutableHashTree(int hash,
                                  TKey key,
                                  TValue value,
                                  ImmutableList<KV<TKey, TValue>> conflicts,
                                  ImmutableHashTree<TKey, TValue> left,
                                  ImmutableHashTree<TKey, TValue> right)
        {
            Hash = hash;
            Key = key;
            Value = value;

            Conflicts = conflicts;

            Left = left;
            Right = right;

            Height = 1 + (left.Height > right.Height ? left.Height : right.Height);
        }

        private ImmutableHashTree<TKey, TValue> InternalAdd(int hashCode, TKey key, TValue value, UpdateDelegate updateDelegate)
        {
            if (Height == 0)
            {
                return new ImmutableHashTree<TKey, TValue>(hashCode,
                                                           key,
                                                           value,
                                                           ImmutableList<KV<TKey, TValue>>.Empty,
                                                           Empty,
                                                           Empty);
            }

            if (hashCode == Hash)
            {
                return ResolveConflicts(key, value, updateDelegate);
            }

            return hashCode < Hash ?
                   New(Left.InternalAdd(hashCode, key, value, updateDelegate), Right).EnsureBalanced() :
                   New(Left, Right.InternalAdd(hashCode, key, value, updateDelegate)).EnsureBalanced();
        }

        private ImmutableHashTree<TKey, TValue> EnsureBalanced()
        {
            int heightDeleta = Left.Height - Right.Height;

            if (heightDeleta > 2)
            {
                ImmutableHashTree<TKey, TValue> newLeft = Left;

                if (Left.Right.Height - Left.Left.Height == 1)
                {
                    newLeft = Left.RotateLeft();
                }

                return New(newLeft, Right).RotateRight();
            }

            if (heightDeleta < -2)
            {
                ImmutableHashTree<TKey, TValue> newRight = Right;

                if (Right.Left.Height - Right.Right.Height == 1)
                {
                    newRight = Right.RotateRight();
                }

                return New(Left, newRight).RotateLeft();
            }

            return this;
        }

        private ImmutableHashTree<TKey, TValue> RotateRight()
        {
            return Left.New(Left.Left, New(Left.Right, Right));
        }

        private ImmutableHashTree<TKey, TValue> RotateLeft()
        {
            return Right.New(New(Left, Right.Left), Right.Right);
        }

        private ImmutableHashTree<TKey, TValue> ResolveConflicts(TKey key, TValue value, UpdateDelegate updateDelegate)
        {
            if (ReferenceEquals(Key, key) || Key.Equals(key))
            {
                TValue newValue = updateDelegate(Value, value);

                return new ImmutableHashTree<TKey, TValue>(Hash, key, newValue, Conflicts, Left, Right);
            }

            return new ImmutableHashTree<TKey, TValue>(Hash, Key, Value, Conflicts.Add(new KV<TKey, TValue>(key, value)), Left, Right);
        }

        private ImmutableHashTree<TKey, TValue> New(ImmutableHashTree<TKey, TValue> left,
                                                    ImmutableHashTree<TKey, TValue> right)
        {
            return new ImmutableHashTree<TKey, TValue>(Hash, Key, Value, Conflicts, left, right);
        }
        
        private static TValue KeyAlreadyExists(TValue currentValue, TValue newValue)
        {
            throw new KeyExistsException<TKey>();
        }
    }
}
