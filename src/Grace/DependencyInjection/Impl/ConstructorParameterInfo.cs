using System;

namespace Grace.DependencyInjection.Impl
{

    /// <summary>
    /// Information about a constructor arguement
    /// </summary>
    public class ConstructorParameterInfo
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConstructorParameterInfo(object exportFunc)
        {
            ExportFunc = exportFunc;
            IsRequired = true;
        }

        /// <summary>
        /// Func&lt;IExportLocator,IExtraDataProvider,TParam&gt;
        /// </summary>
        public object ExportFunc { get; set; }

        /// <summary>
        /// Name of the arguement 
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Type of the arguement 
        /// </summary>
        public Type ParameterType { get; set; }
        
        /// <summary>
        /// Is the import required
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue { get; set; }
    
        /// <summary>
        /// Export filter associated with this import
        /// </summary>
        public ActivationStrategyFilter ExportStrategyFilter { get; set; }

        /// <summary>
        /// Comparer to use on enumerable
        /// </summary>
        public object EnumerableComparer { get; set; }

        /// <summary>
        /// Locate parameter with key
        /// </summary>
        public object LocateWithKey { get; set; }

        /// <summary>
        /// Is the parameter dynamic
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Use specific type for parameter
        /// </summary>
        public Type UseType { get; set; }
    }
}
