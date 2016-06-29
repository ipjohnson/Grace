using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection
{
    public class KernelConfiguration : IKernelConfiguration
    {
        private Func<IInjectionScope, Type, ConstructorInfo> _constructorPicker;
        private Func<IDisposalScope, IInjectionScope, IInjectionContext> _contextCreation;
        private Func<IInjectionScope, Type, ICompiledExportStrategy> _exportStrategyProvider;
        private Func<IInjectionScope, IExportRegistrationBlockStrategyProvider> _registrationBlockCreation;
        private Func<IInjectionScope, Type, ICompiledExportStrategy> _closedGenericProvider;

        public bool AutoRegisterUnknown { get; set; }

        public Func<Type, bool> BlackListFilter { get; set; }

        public Func<IInjectionScope, Type, ICompiledExportStrategy> ClosedGenericExportStrategyProvider
        {
            get
            {
                if(_closedGenericProvider == null)
                {
                    _closedGenericProvider = GenericExportStrategy.CreateClosedGenericExportStrategy;
                }

                return _closedGenericProvider;
            }
            set { _closedGenericProvider = value; }
        }

        public ExportStrategyComparer Comparer { get; set; }

        public Func<IInjectionScope, Type, ConstructorInfo> ConstructorPicker
        {
            get
            {
                if(_constructorPicker == null)
                {
                    _constructorPicker = InstanceCompiledExportDelegate.DefaultConstructorPicker;
                }

                return _constructorPicker;
            }
            set { _constructorPicker = value; }
        }

        public Func<IDisposalScope, IInjectionScope, IInjectionContext> ContextCreation
        {
            get
            {
                if(_contextCreation == null)
                {
                    _contextCreation = InjectionContext.DefaultCreateContext;
                }

                return _contextCreation;
            }
            set { _contextCreation = value; }
        }

        public IDisposalScopeProvider DisposalScopeProvider { get; set; }

        public Func<IInjectionScope, Type, ICompiledExportStrategy> ExportStrategyProvider
        {
            get
            {
                if(_exportStrategyProvider == null)
                {
                    _exportStrategyProvider = ExportRegistrationBlock.DefaultExportStrategyProvider;
                }

                return _exportStrategyProvider;
            }
            set { _exportStrategyProvider = value; }
                
        }

        public Func<IInjectionScope, IExportRegistrationBlockStrategyProvider> RegistrationBlockCreation
        {
            get
            {
                if(_registrationBlockCreation == null)
                {
                    _registrationBlockCreation = ExportRegistrationBlock.DefaultRegistrationBlockCreation;
                }

                return _registrationBlockCreation;
            }
            set { _registrationBlockCreation = value; }
        }

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
