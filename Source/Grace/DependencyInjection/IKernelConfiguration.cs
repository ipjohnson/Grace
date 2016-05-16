using Grace.DependencyInjection.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    public interface IKernelConfiguration
    {
        Func<Type, bool> BlackListFilter { get; set; }

        Func<IInjectionScope, Type, ConstructorInfo> ConstructorPicker { get; set; }

        Func<IInjectionScope, Type, ICompiledExportStrategy> ExportStrategyProvider { get; set; }

        ExportStrategyComparer Comparer { get; set; }

        Func<IInjectionScope, Type, IInjectionContext> ContextCreation { get; set; }

        Func<IInjectionScope, IExportRegistrationBlock> RegistrationBlockCreation { get; set; }

        IDisposalScopeProvider DisposalScopeProvider { get; set; } 
        
        bool AutoRegisterUnknown { get; set; }

        IKernelConfiguration Clone();
    }
}
