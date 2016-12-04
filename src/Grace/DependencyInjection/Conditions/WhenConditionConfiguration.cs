using System;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// Class that allows the configuration of conditions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhenConditionConfiguration<T> : IWhenConditionConfiguration<T>
    {
        private readonly Action<ICompiledCondition> _addAction;
        private readonly T _t;

        /// <summary>
        /// Default constructor that takes action to add condition and T to return
        /// </summary>
        /// <param name="addAction">action to add condition</param>
        /// <param name="t">T to return</param>
        public WhenConditionConfiguration(Action<ICompiledCondition> addAction, T t)
        {
            if (addAction == null) throw new ArgumentNullException(nameof(addAction));

            if (t == null) throw new ArgumentNullException(nameof(t));

            _addAction = addAction;

            _t = t;
        }

        /// <summary>
        /// Adds a new compiled condition to the strategy
        /// </summary>
        /// <param name="condition">condition to add</param>
        /// <returns>T</returns>
        public T MeetsCondition(ICompiledCondition condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            _addAction(condition);

            return _t;
        }

        /// <summary>
        /// Adds a condition to strategy that is a function with the signature (strategy, staticContext)
        /// </summary>
        /// <param name="condition">test condition</param>
        /// <returns></returns>
        public T MeetsCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            _addAction(new FuncCompiledCondition(condition));

            return _t;
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

            _addAction(new WhenTargetHas(typeof(TAttribute), func));

            return _t;
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

            _addAction(new WhenClassHas(attributeType, testFunc));

            return _t;
        }

        /// <summary>
        /// Class being injected into has a specific attribute
        /// </summary>
        /// <param name="testFunc"></param>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public T ClassHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute
        {
            _addAction(new WhenClassHas<TAttribute>(testFunc));

            return _t;
        }

        /// <summary>
        /// Add a condition to use this export only when the member being injected into has a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        public T MemberHas<TAttribute>(Func<TAttribute, bool> testFunc = null) where TAttribute : Attribute
        {
            _addAction(new WhenMemberHas<TAttribute>(testFunc));

            return _t;
        }

        /// <summary>
        /// Use strategy when injected into a specific type
        /// </summary>
        /// <typeparam name="TInjectedType">injected type</typeparam>
        /// <returns></returns>
        public T InjectedInto<TInjectedType>()
        {
            _addAction(new WhenInjectedInto(typeof(TInjectedType)));

            return _t;
        }

        /// <summary>
        /// Use strategy when injected into a specific type
        /// </summary>
        /// <param name="types">types allowed to be injected into</param>
        /// <returns></returns>
        public T InjectedInto(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            if (types.Length < 0) throw new ArgumentException("You must provide at least one type to test against", nameof(types));

            _addAction(new WhenInjectedInto(types));

            return _t;
        }
    }
}
