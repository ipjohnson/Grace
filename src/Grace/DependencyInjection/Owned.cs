using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    public class Owned<T> : DisposalScope where T : class
    {
        public T Value { get; private set; }

        public Owned<T> SetValue(T value)
        {
            if (Value == null)
            {
                Value = value;
            }

            return this;
        }
    }
}
