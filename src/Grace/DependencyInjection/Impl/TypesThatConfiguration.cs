using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class TypesThatConfiguration : GenericFilterGroup<Type>
    {
        /// <summary>
        /// Creates a new type filter based on the types namespace
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="includeSubnamespaces"></param>
        /// <returns></returns>
        public TypesThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
        {
            bool notValue = GetNotAndingValue();
            Func<Type, bool> newFilter;

            if (includeSubnamespaces)
            {
                newFilter = type => (type.Namespace == @namespace ||
                                         (type.Namespace != null &&
                                          type.Namespace.StartsWith(@namespace + "."))) == notValue;
            }
            else
            {
                newFilter = type => (type.Namespace == @namespace) == notValue;
            }

            Add(newFilter);

            return this;
        }

        /// <summary>
        /// Creates a new type filter that fiters based on if it's in the same namespace as another class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeSubnamespaces"></param>
        /// <returns></returns>
        public TypesThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
        {
            return AreInTheSameNamespace(type.Namespace, includeSubnamespaces);
        }

        /// <summary>
        /// Creates a new type filter that fiters based on if it's in the same namespace as another class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeSubnamespaces"></param>
        /// <returns></returns>
        public TypesThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
        {
            return AreInTheSameNamespaceAs(typeof(T), includeSubnamespaces);
        }


        /// <summary>
        /// Reverses the logic for the next type filter
        /// </summary>
        public TypesThatConfiguration Not
        {
            get
            {
                notLogicValue = false;

                return this;
            }
        }

        private bool notLogicValue = true;

        private bool GetNotAndingValue()
        {
            bool tempValue = notLogicValue;

            notLogicValue = true;

            return tempValue;
        }
    }
}
