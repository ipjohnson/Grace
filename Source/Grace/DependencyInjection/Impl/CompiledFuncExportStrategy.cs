using System.Collections.Generic;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This export takes an activation delegate and creates a linq compiled expression
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CompiledFuncExportStrategy<T> : CompiledExportStrategy
	{
		private IEnumerable<ExportStrategyDependency> dependencies;
		private readonly ExportActivationDelegate exportActivationDelegate;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportActivationDelegate"></param>
		public CompiledFuncExportStrategy(ExportActivationDelegate exportActivationDelegate)
			: base(typeof(T))
		{
			this.exportActivationDelegate = exportActivationDelegate;
		}
        
        /// <summary>
        /// List of dependencies for this strategy
        /// </summary>
        public override IEnumerable<ExportStrategyDependency> DependsOn
        {
            get
            {
                if (dependencies == null)
                {
                    LazyInitialize();
                }

                return dependencies;
            }
        }

        protected override void LazyInitialize()
        {
            if(activationDelegate == null)
            {
                lock(exportActivationDelegate)
                {
                    if(activationDelegate == null)
                    {
                        CompiledExportDelegateInfo info = GetCompiledInfo();

                        FuncCompiledExportDelegate delegateGenerator =
                            new FuncCompiledExportDelegate(info, exportActivationDelegate, this, OwningScope);

                        activationDelegate = delegateGenerator.CompileDelegate();

                        dependencies = delegateGenerator.Dependencies;
                    }
                }
            }            
        }
    }
}