using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data.Immutable
{
    public class ImmutableStack<T> : IEnumerable<T>
    {
        private readonly T[] _stack;

        /// <summary>
        /// Empty stack
        /// </summary>
        public static readonly ImmutableStack<T> Empty = new ImmutableStack<T>(); 

        public ImmutableStack(IEnumerable<T> stack)
        {
            _stack = stack.ToArray();
        }

        public ImmutableStack(params T[] stack)
        {
            T[] newArray = new T[stack.Length];

            Array.Copy(stack, 0, newArray, 0, stack.Length);

            _stack = newArray;
        }

        /// <summary>
        /// Constructor is internal to allow for not copying array on construction
        /// </summary>
        /// <param name="privateParam"></param>
        /// <param name="stack"></param>
        private ImmutableStack(bool privateParam, T[] stack)
        {
            _stack = stack;
        }

        public ImmutableStack<T> Pop()
        {
            if (_stack.Length == 0)
            {
                throw new Exception("Stack is empty");
            }

            T[] newArray = new T[_stack.Length - 1];

            Array.Copy(_stack, 0, newArray, 0, _stack.Length - 1);

            return new ImmutableStack<T>(false, newArray);
        }

        public ImmutableStack<T> Pop(out T value)
        {
            if (_stack.Length == 0)
            {
                throw new Exception("Stack is empty");
            }

            T[] newArray = new T[_stack.Length - 1];

            Array.Copy(_stack, 0, newArray, 0, _stack.Length - 1);

            value = _stack[_stack.Length - 1];

            return new ImmutableStack<T>(false, newArray);
        }

        public ImmutableStack<T> Push(T value)
        {
            int stackLength = _stack.Length;
            T[] newArray = new T[stackLength + 1];

            Array.Copy(_stack, 0, newArray, 0, stackLength);

            newArray[stackLength] = value;

            return new ImmutableStack<T>(false, newArray);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ImmutableArrayEnumerator<T>(_stack);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
