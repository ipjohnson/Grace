using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerNamedScopeLifestyle : ILifestyle
    {
        private readonly string _namedScope;
        private readonly string _uniqueId = Guid.NewGuid().ToString();
        private readonly object _lockObject = new object();

        public SingletonPerNamedScopeLifestyle(string namedScope)
        {
            _namedScope = namedScope;
        }

        public void Dispose()
        {
            // don't do anything all disposal of objects created is handled by the named scope
        }

        public bool Transient
        {
            get { return false; }
        }

        public object Locate(ExportActivationDelegate creationDelegate,
            IInjectionScope injectionScope,
            IInjectionContext injectionContext,
            IExportStrategy exportStrategy)
        {
            object returnValue = null;
            IInjectionScope locateScope = FindNamedScope(injectionContext.RequestingScope);

            if (locateScope == null)
            {
                throw new InjectionScopeCouldNotBeFoundException(_namedScope);
            }

            returnValue = locateScope.GetExtraData(_uniqueId);

            if (returnValue != null)
            {
                return returnValue;
            }

            lock (_lockObject)
            {
                returnValue = locateScope.GetExtraData(_uniqueId) ??
                              SingletonLifestyle.CreateInSingletonScope(creationDelegate,
                                  locateScope,
                                  injectionContext);

                if (returnValue != null)
                {
                    locateScope.SetExtraData(_uniqueId, returnValue);

                    IDisposable disposable = returnValue as IDisposable;

                    if (disposable != null)
                    {
                        locateScope.AddDisposable(disposable);
                    }
                }
            }

            return returnValue;
        }

        public ILifestyle Clone()
        {
            return new SingletonPerNamedScopeLifestyle(_namedScope);
        }

        private IInjectionScope FindNamedScope(IInjectionScope scope)
        {
            IInjectionScope currentScope = scope;

            while (currentScope != null)
            {
                if (currentScope.ScopeName == _namedScope)
                {
                    break;
                }

                currentScope = scope.ParentScope;
            }

            return currentScope;
        }
    }
}
