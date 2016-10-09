using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{

    public interface IAttributeDiscoveryService
    {
        IEnumerable<Attribute> GetAttributes(object value);
    }

    public class AttributeDiscoveryService : IAttributeDiscoveryService
    {
        private ImmutableHashTree<object, IEnumerable<Attribute>> _knownValues =
            ImmutableHashTree<object, IEnumerable<Attribute>>.Empty;

        public IEnumerable<Attribute> GetAttributes(object value)
        {
            if (value == null)
            {
                return ImmutableLinkedList<Attribute>.Empty;
            }

            var values = _knownValues.GetValueOrDefault(value);

            if (values != null)
            {
                return values;
            }

            Attribute[] attributes = null;

            var type = value as Type;

            if (type != null)
            {
                attributes = type.GetTypeInfo().GetCustomAttributes().ToArray();
            }
            else
            {
                var parameterInfo = value as ParameterInfo;

                if (parameterInfo != null)
                {
                    attributes = parameterInfo.GetCustomAttributes().ToArray();
                }
                else
                {
                    var fieldInfo = value as FieldInfo;

                    if (fieldInfo != null)
                    {
                        attributes = fieldInfo.GetCustomAttributes().ToArray();
                    }
                    else
                    {
                        var methodInfo = value as MethodInfo;

                        if (methodInfo != null)
                        {
                            attributes = methodInfo.GetCustomAttributes().ToArray();
                        }
                        else
                        {
                            throw new NotSupportedException(
                                $"Getting attributes on type {value.GetType().Name} is not supported");
                        }
                    }
                }
            }

            if (attributes.Length == 0)
            {
                return ImmutableLinkedList<Attribute>.Empty;
            }

            var list = ImmutableLinkedList.From(attributes);

            return ImmutableHashTree.ThreadSafeAdd(ref _knownValues, value, list);
        }
    }
}
