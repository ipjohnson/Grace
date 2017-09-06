using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Interface for creating strongly type metadata instances
    /// </summary>
    public interface IStrongMetadataInstanceProvider
    {
        /// <summary>
        /// Create new instance of metadata type using provided metadata
        /// </summary>
        /// <param name="metadataType"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        object GetMetadata(Type metadataType, IActivationStrategyMetadata metadata);
    }

    /// <summary>
    /// Class to create strongly typed instances
    /// </summary>
    public class StrongMetadataInstanceProvider : IStrongMetadataInstanceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataType"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public virtual object GetMetadata(Type metadataType, IActivationStrategyMetadata metadata)
        {
            if (metadataType == typeof(IReadOnlyDictionary<object, object>) ||
                metadataType == typeof(IActivationStrategyMetadata))
            {
                return metadata;
            }

            if (metadataType.GetTypeInfo().IsInterface)
            {
                throw new NotSupportedException("Interface metadata types not supported");
            }

            var constructorParameters = GetConstructorParameters(metadataType, metadata);

            var instance = Activator.CreateInstance(metadataType, constructorParameters);

            BindPropertyValues(instance, metadata);

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataType"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected object[] GetConstructorParameters(Type metadataType, IActivationStrategyMetadata metadata)
        {
            var constructors = metadataType.GetTypeInfo().DeclaredConstructors.ToArray();

            ConstructorInfo constructorInfo;

            if (constructors.Length == 1)
            {
                if (constructors[0].GetParameters().Length == 0)
                {
                    return new object[0];
                }

                constructorInfo = constructors[0];
            }
            else
            {
                constructorInfo = constructors.OrderBy(c => c.GetParameters().Length).Last();
            }

            var parameters = constructorInfo.GetParameters();

            var constructorParameters = new object[parameters.Length];

            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(IActivationStrategyMetadata) ||
                    parameter.ParameterType == typeof(IReadOnlyDictionary<object, object>))
                {
                    constructorParameters[index] = metadata;
                }

                var uppercaseName = char.ToUpper(parameter.Name[0]).ToString();

                if (parameter.Name.Length > 1)
                {
                    uppercaseName += parameter.Name.Substring(1);
                }

                if (metadata.TryGetValue(parameter.Name, out object parameterValue) ||
                    metadata.TryGetValue(uppercaseName, out parameterValue))
                {
                    if (parameterValue != null)
                    {
                        constructorParameters[index] =
                            parameter.ParameterType.GetTypeInfo().IsAssignableFrom(parameterValue.GetType().GetTypeInfo())
                                ? parameterValue
                                : ConvertValue(parameter.ParameterType, parameterValue);
                    }
                    else
                    {
                        constructorParameters[index] = null;
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    constructorParameters[index] = parameter.DefaultValue;
                }
                else
                {
                    constructorParameters[index] = parameter.ParameterType.GetTypeInfo().IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null;
                }
            }

            return constructorParameters;
        }

        /// <summary>
        /// Bind metadata values to instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="metadata"></param>
        protected void BindPropertyValues(object instance, IActivationStrategyMetadata metadata)
        {
            foreach (var propertyInfo in instance.GetType().GetRuntimeProperties())
            {
                if (!propertyInfo.CanWrite ||
                    propertyInfo.SetMethod.IsStatic)
                {
                    continue;
                }

                object setValue = null;

                setValue = metadata.ContainsKey(propertyInfo.Name) ?
                            metadata[propertyInfo.Name] :
                            propertyInfo.GetCustomAttribute<DefaultValueAttribute>()?.Value;

                if (setValue != null)
                {
                    if (!propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(setValue.GetType().GetTypeInfo()))
                    {
                        setValue = ConvertValue(propertyInfo.PropertyType, setValue);
                    }

                    propertyInfo.SetMethod.Invoke(instance, new[] { setValue });
                }
            }
        }

        protected virtual object ConvertValue(Type desiredType, object value)
        {
            return Convert.ChangeType(value, desiredType);
        }
    }
}
