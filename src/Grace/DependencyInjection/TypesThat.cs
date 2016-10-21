using System;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    public static class TypesThat
    {
        public static TypesThatConfiguration AreInTheSameNamespace(string @namespace, bool includeSubnamespaces = false)
        {
            return new TypesThatConfiguration().AreInTheSameNamespace(@namespace, includeSubnamespaces);
        }

        public static TypesThatConfiguration AreInTheSameNamespaceAs(Type type, bool includeSubnamespaces = false)
        {
            return new TypesThatConfiguration().AreInTheSameNamespaceAs(type, includeSubnamespaces);
        }

        public static TypesThatConfiguration AreInTheSameNamespaceAs<T>(bool includeSubnamespaces = false)
        {
            return new TypesThatConfiguration().AreInTheSameNamespaceAs<T>(includeSubnamespaces);
        }
    }
}
