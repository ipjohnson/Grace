using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// class that holds dependency and acts as a disposal scope for it and it's children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Owned<T> : DisposalScope where T : class
    {
        /// <summary>
        /// Resolved dependency
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Set owned value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
