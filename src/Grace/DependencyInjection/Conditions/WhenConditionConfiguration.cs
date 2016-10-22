using System;

namespace Grace.DependencyInjection.Conditions
{
    public class WhenConditionConfiguration<T> : IWhenConditionConfiguration<T>
    {
        private readonly Action<ICompiledCondition> _addAction;
        private readonly T _t;

        public WhenConditionConfiguration(Action<ICompiledCondition> addAction, T t)
        {
            _addAction = addAction;
            _t = t;
        }
        
        public T MeetsCondition(ICompiledCondition condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            _addAction(condition);

            return _t;
        }

        public T MeetsCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            _addAction(new FuncCompiledCondition(condition));

            return _t;
        }

        public T ClassHas<TAttribute>()
        {
            throw new NotImplementedException();
        }

        public T InjectedInto<TInjectedType>()
        {
            _addAction(new WhenInjectedInto(typeof(TInjectedType)));

            return _t;
        }

        public T InjectedInto(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            if (types.Length < 0) throw new ArgumentException("", nameof(types));

            _addAction(new WhenInjectedInto());

            return _t;
        }
    }
}
