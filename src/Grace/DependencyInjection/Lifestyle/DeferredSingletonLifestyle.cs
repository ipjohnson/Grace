namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton lifestyle that defers the creation of the instance till requested.
    /// The standard singleton lifestyle creates the instance early. 
    /// </summary>
    public class DeferredSingletonLifestyle : SingletonLifestyle
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DeferredSingletonLifestyle() : base(true)
        { }

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        public override ICompiledLifestyle Clone() => new DeferredSingletonLifestyle();
    }
}
