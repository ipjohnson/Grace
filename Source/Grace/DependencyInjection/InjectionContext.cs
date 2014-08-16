using System;
using System.Collections;
using System.Collections.Generic;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Default implementation of IInjectionContext
    /// </summary>
    public class InjectionContext : IInjectionContext, IEnumerable<KeyValuePair<string, ExportActivationDelegate>>
    {
        private Dictionary<string, ExportActivationDelegate> exportsByName;
        private Dictionary<Type, ExportActivationDelegate> exportsByType;
        private Dictionary<string, object> extraData;
        private int resolveDepth;
        private CurrentInjectionInfo[] currentInjectionInfo;

        /// <summary>
        /// Constructor that uses requesting scope as disposal scope
        /// </summary>
        /// <param name="requestingScope"></param>
        public InjectionContext(IInjectionScope requestingScope) :
            this(requestingScope, requestingScope)
        {
            currentInjectionInfo = new CurrentInjectionInfo[4];
            MaxResolveDepth = 50;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="disposalScope"></param>
        /// <param name="requestingScope"></param>
        public InjectionContext(IDisposalScope disposalScope, IInjectionScope requestingScope)
        {
            currentInjectionInfo = new CurrentInjectionInfo[4];
            MaxResolveDepth = 50;

            DisposalScope = disposalScope ?? requestingScope;

            RequestingScope = requestingScope;
        }

        /// <summary>
        /// Returns an enumeration of exports
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, ExportActivationDelegate>> GetEnumerator()
        {
            if (exportsByName != null)
            {
                foreach (KeyValuePair<string, ExportActivationDelegate> exportActivationDelegate in exportsByName)
                {
                    yield return exportActivationDelegate;
                }
            }
        }

        /// <summary>
        /// Gets an enumation of exports
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clone the context
        /// </summary>
        /// <returns></returns>
        public IInjectionContext Clone()
        {
            InjectionContext injectionContext = new InjectionContext(DisposalScope, RequestingScope);

            if (exportsByName != null)
            {
                injectionContext.exportsByName = new Dictionary<string, ExportActivationDelegate>(exportsByName);
            }

            if (exportsByType != null)
            {
                injectionContext.exportsByType = new Dictionary<Type, ExportActivationDelegate>(exportsByType);
            }

            if (extraData != null)
            {
                injectionContext.extraData = new Dictionary<string, object>(extraData);
            }

            injectionContext.TargetInfo = TargetInfo;
            injectionContext.resolveDepth = resolveDepth;
            injectionContext.RequestingScope = RequestingScope;
            injectionContext.DisposalScope = DisposalScope;

            injectionContext.currentInjectionInfo = new CurrentInjectionInfo[currentInjectionInfo.Length];

            injectionContext.currentInjectionInfo.CopyTo(currentInjectionInfo, 0);

            return injectionContext;
        }

        /// <summary>
        /// Disposal scope for the injection context
        /// </summary>
        public IDisposalScope DisposalScope { get; set; }

        /// <summary>
        /// The scope that the request originated in
        /// </summary>
        public IInjectionScope RequestingScope { get; set; }

        /// <summary>
        /// The target information for the current injection
        /// </summary>
        public IInjectionTargetInfo TargetInfo { get; set; }

        /// <summary>
        /// When importing a property after construction this will contain the instance that is being injected
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// Extra data associated with the injection request. 
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public object GetExtraData(string dataName)
        {
            object returnValue = null;

            if (extraData != null)
            {
                extraData.TryGetValue(dataName, out returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Sets extra data on the injection context
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="newValue"></param>
        public void SetExtraData(string dataName, object newValue)
        {
            if (extraData == null)
            {
                extraData = new Dictionary<string, object>();
            }

            extraData[dataName] = newValue;
        }

        /// <summary>
        /// Locate an export by type
        /// </summary>
        /// <returns></returns>
        public object Locate<T>()
        {
            if (exportsByType != null)
            {
                ExportActivationDelegate activationDelegate;

                if (exportsByType.TryGetValue(typeof(T), out activationDelegate))
                {
                    return activationDelegate(RequestingScope, this);
                }
            }

            return null;
        }

        /// <summary>
        /// Locate an export by type
        /// </summary>
        /// <returns></returns>
        public object Locate(Type type)
        {
            if (exportsByType != null)
            {
                ExportActivationDelegate activationDelegate;

                if (exportsByType.TryGetValue(type, out activationDelegate))
                {
                    return activationDelegate(RequestingScope, this);
                }
            }

            return null;
        }

        /// <summary>
        /// Locate an export by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Locate(string name)
        {
            if (exportsByName != null)
            {
                ExportActivationDelegate activationDelegate;

                if (exportsByName.TryGetValue(name.ToLowerInvariant(), out activationDelegate))
                {
                    return activationDelegate(RequestingScope, this);
                }
            }

            return null;
        }

        /// <summary>
        /// Register an export by type for this injection context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportFunction"></param>
        public void Export<T>(ExportFunction<T> exportFunction)
        {
            Export(typeof(T), (x, y) => exportFunction(x, y));
        }

        /// <summary>
        /// Export by type
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="activationDelegate"></param>
        public void Export(Type exportType, ExportActivationDelegate activationDelegate)
        {
            if (exportsByType == null)
            {
                exportsByType = new Dictionary<Type, ExportActivationDelegate>();
            }

            exportsByType[exportType] = activationDelegate;
        }

        /// <summary>
        /// Register an export by name for this injection context
        /// </summary>
        /// <param name="name"></param>
        /// <param name="activationDelegate"></param>
        public void Export(string name, ExportActivationDelegate activationDelegate)
        {
            name = name.ToLowerInvariant();

            if (exportsByName == null)
            {
                exportsByName = new Dictionary<string, ExportActivationDelegate>();
            }

            exportsByName[name] = activationDelegate;
        }

        /// <summary>
        /// Max resolve depth allowed
        /// </summary>
        public int MaxResolveDepth { get; set; }

        /// <summary>
        /// Push a current export strategy onto the stack
        /// </summary>
        /// <param name="exportStrategy">export strategy</param>
        public void PushCurrentInjectionInfo<T>(IExportStrategy exportStrategy)
        {
            if (resolveDepth > MaxResolveDepth)
            {
                if (TargetInfo != null)
                {
                    throw new CircularDependencyDetectedException(TargetInfo.LocateName, TargetInfo.LocateType, this);
                }

                throw new CircularDependencyDetectedException(null, (Type)null, this);
            }

            if (resolveDepth >= currentInjectionInfo.Length)
            {
                var temp = new CurrentInjectionInfo[currentInjectionInfo.Length * 2];

                currentInjectionInfo.CopyTo(temp, 0);

                currentInjectionInfo = temp;
            }

            currentInjectionInfo[resolveDepth] = new CurrentInjectionInfo(typeof(T), exportStrategy);

            resolveDepth++;
        }

        /// <summary>
        /// Pop the current export strategy off the stack
        /// </summary>
        public void PopCurrentInjectionInfo()
        {
            resolveDepth--;
        }

        /// <summary>
        /// Injection info all the way up the stack
        /// </summary>
        /// <returns></returns>
        public CurrentInjectionInfo[] GetInjectionStack()
        {
            CurrentInjectionInfo[] returnValue = new CurrentInjectionInfo[resolveDepth];

            Array.Copy(currentInjectionInfo, 0, returnValue, 0, resolveDepth);

            return returnValue;
        }

        /// <summary>
        /// Add a new object to injection context for export
        /// </summary>
        /// <param name="export"></param>
        public void Add(object export)
        {
            Export(export.GetType(), (x, y) => export);
        }

        /// <summary>
        /// Add a new Type to injection context for export
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportFunc"></param>
        public void Add<T>(ExportFunction<T> exportFunc)
        {
            Export(exportFunc);
        }

        /// <summary>
        /// Adds a new export by name to the injection context
        /// </summary>
        /// <param name="name">export name</param>
        /// <param name="value">export value</param>
        public void Add(string name, object value)
        {
            Add(name.ToLowerInvariant(), (x, y) => value);
        }

        /// <summary>
        /// Adds a new export by name to the injection context
        /// </summary>
        /// <param name="name">export name</param>
        /// <param name="activationDelegate">activation delegate</param>
        public void Add(string name, ExportActivationDelegate activationDelegate)
        {
            Export(name.ToLowerInvariant(), activationDelegate);
        }
    }
}