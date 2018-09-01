using System;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// Class that allows the configuration of conditions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhenConditionConfiguration<T> : IWhenConditionConfiguration<T>
    {
        /// <summary>
        /// Add action
        /// </summary>
        protected readonly Action<ICompiledCondition> AddAction;

        /// <summary>
        /// TValue to return
        /// </summary>
        protected readonly T TValue;

        /// <summary>
        /// Default constructor that takes action to add condition and T to return
        /// </summary>
        /// <param name="addAction">action to add condition</param>
        /// <param name="t">T to return</param>
        public WhenConditionConfiguration(Action<ICompiledCondition> addAction, T t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));

            AddAction = addAction ?? throw new ArgumentNullException(nameof(addAction));

            TValue = t;
        }

        /// <summary>
        /// Adds a new compiled condition to the strategy
        /// </summary>
        /// <param name="condition">condition to add</param>
        /// <returns>T</returns>
        public T MeetsCondition(ICompiledCondition condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            AddAction(condition);

            return TValue;
        }

        /// <summary>
        /// Adds a condition to strategy that is a function with the signature (strategy, staticContext)
        /// </summary>
        /// <param name="condition">test condition</param>
        /// <returns></returns>
        public T MeetsCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            AddAction(new FuncCompiledCondition(condition));

            return TValue;
        }

        /// <summary>
        /// Add a condition to use this export only when Target (parameter, property, method) has an attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public T TargetHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute
        {
            Func<Attribute, bool> func = null;

            if (testFunc != null)
            {
                func = attr => testFunc((TAttribute) attr);
            }

            AddAction(new WhenTargetHas(typeof(TAttribute), func));

            return TValue;
        }

        /// <summary>
        /// Add a condition to use this export only when the class being injected into has a specific attribute
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        public T ClassHas(Type attributeType, Func<Attribute, bool> testFunc = null)
        {
            if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));

            AddAction(new WhenClassHas(attributeType, testFunc));

            return TValue;
        }

        /// <summary>
        /// Class being injected into has a specific attribute
        /// </summary>
        /// <param name="testFunc"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public T ClassHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute
        {
            AddAction(new WhenClassHas<TAttribute>(testFunc));

            return TValue;
        }

        /// <summary>
        /// Add a condition to use this export only when the member being injected into has a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        public T MemberHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute
        {
            AddAction(new WhenMemberHas<TAttribute>(testFunc));

            return TValue;
        }

        /// <summary>
        /// Use strategy when injected into a specific type
        /// </summary>
        /// <typeparam name="TInjectedType">injected type</typeparam>
        /// <returns></returns>
        public T InjectedInto<TInjectedType>()
        {
            AddAction(new WhenInjectedInto(typeof(TInjectedType)));

            return TValue;
        }

        /// <summary>
        /// Use strategy when injected into a specific type
        /// </summary>
        /// <param name="types">types allowed to be injected into</param>
        /// <returns></returns>
        public T InjectedInto(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            if (types.Length < 1) throw new ArgumentException("You must provide at least one type to test against", nameof(types));

            AddAction(new WhenInjectedInto(types));

            return TValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
        public T InjectedInto(Func<Type, bool> consider)
        {
            if (consider == null) throw new ArgumentNullException(nameof(consider));

            AddAction(new WhenInjectedInto(consider));

            return TValue;
        }
    }
}
