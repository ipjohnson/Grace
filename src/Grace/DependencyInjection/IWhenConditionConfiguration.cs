using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Object used to configure a condition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWhenConditionConfiguration<out T>
    {
        /// <summary>
        /// Use strategy when it meets condition
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        T MeetsCondition(ICompiledCondition condition);

        /// <summary>
        /// Use strategy when it meets a condition
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        T MeetsCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition);

        /// <summary>
        /// Add a condition to use this export only when Target (parameter, property, method) has an attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        T TargetHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute;

        /// <summary>
        /// Add a condition to use this export only when the class being injected into has a specific attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        T ClassHas(Type attributeType, Func<Attribute, bool> testFunc = null);

        /// <summary>
        /// Add a condition to use this export only when the class being injected into has a specific attribute
        /// </summary>
        /// <param name="testFunc"></param>
        /// <typeparam name="TAttribute">attribute</typeparam>
        /// <returns></returns>
        T ClassHas<TAttribute>(Func<TAttribute,bool> testFunc = null) where TAttribute : Attribute; 

        /// <summary>
        /// Add a condition to use this export only when the member being injected into has a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        T MemberHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute;

        /// <summary>
        /// Use export when injecting into a specific type
        /// </summary>
        /// <typeparam name="TInjectedType">injected type</typeparam>
        /// <returns>configuration</returns>
        T InjectedInto<TInjectedType>();

        /// <summary>
        /// Use export when injected into one of the specified types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        T InjectedInto(params Type[] types);

        /// <summary>
        /// Injected into type using test method
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
        T InjectedInto(Func<Type, bool> consider);
    }
}
