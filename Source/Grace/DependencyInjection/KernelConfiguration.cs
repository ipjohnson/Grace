using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    public class KernelConfiguration : IKernelConfiguration
    {
        public bool AutoRegisterUnknown { get; set; }

        public Func<Type, bool> BlackListFilter { get; set; }

        public ExportStrategyComparer Comparer { get; set; }

        public Func<IInjectionScope, Type, ConstructorInfo> ConstructorPicker { get; set; }

        public Func<IInjectionScope, Type, IInjectionContext> ContextCreation { get; set; }

        public IDisposalScopeProvider DisposalScopeProvider { get; set; }

        public Func<IInjectionScope, Type, ICompiledExportStrategy> ExportStrategyProvider { get; set; }

        public Func<IInjectionScope, IExportRegistrationBlock> RegistrationBlockCreation { get; set; }

        public virtual IKernelConfiguration Clone()
        {
            return new KernelConfiguration
            {
                AutoRegisterUnknown = AutoRegisterUnknown,
                BlackListFilter = BlackListFilter,
                Comparer = Comparer,
                ConstructorPicker = ConstructorPicker,
                ContextCreation = ContextCreation,
                DisposalScopeProvider = DisposalScopeProvider,
                ExportStrategyProvider = ExportStrategyProvider,
                RegistrationBlockCreation = RegistrationBlockCreation
            };
        }
    }
}
