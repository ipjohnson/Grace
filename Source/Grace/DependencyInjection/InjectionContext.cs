using System;
using System.Collections;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Default implementation of IInjectionContext
    /// </summary>
    public class InjectionContext : IInjectionContext, IEnumerable<KeyValuePair<string, ExportActivationDelegate>>
    {
        private ImmutableHashTree<string, ExportActivationDelegate> _exportsByName;
        private ImmutableHashTree<Type, ExportActivationDelegate> _exportsByType;
        private ImmutableHashTree<string, object> _extraData;
        private int _resolveDepth;
        private CurrentInjectionInfo[] _currentInjectionInfo;
        private IDisposalScope _disposalScope;
        private IInjectionScope _requestingScope;
        private IInjectionTargetInfo _targetInfo;

        /// <summary>
        /// Constructor that uses requesting scope as disposal scope
        /// </summary>
        /// <param name="requestingScope"></param>
        public InjectionContext(IInjectionScope requestingScope) :
            this(requestingScope, requestingScope)
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="disposalScope"></param>
        /// <param name="requestingScope"></param>
        public InjectionContext(IDisposalScope disposalScope, IInjectionScope requestingScope)
        {
            _currentInjectionInfo = new CurrentInjectionInfo[4];
            MaxResolveDepth = 50;

            _disposalScope = disposalScope ?? requestingScope;

            _requestingScope = requestingScope;
        }

        /// <summary>
        /// Returns an enumeration of exports
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, ExportActivationDelegate>> GetEnumerator()
        {
            if (_exportsByName != null)
            {
                foreach (KeyValuePair<string, ExportActivationDelegate> exportActivationDelegate in _exportsByName)
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
            InjectionContext injectionContext = new InjectionContext(DisposalScope, RequestingScope)
                                                {
                                                    _exportsByName = _exportsByName,
                                                    _exportsByType = _exportsByType,
                                                    _extraData = _extraData,
                                                    _targetInfo = _targetInfo,
                                                    _resolveDepth = _resolveDepth,
                                                    _requestingScope = _requestingScope,
                                                    _disposalScope = _disposalScope,
                                                    _currentInjectionInfo = new CurrentInjectionInfo[_currentInjectionInfo.Length]
                                                };

            injectionContext._currentInjectionInfo.CopyTo(_currentInjectionInfo, 0);

            return injectionContext;
        }

        /// <summary>
        /// Disposal scope for the injection context
        /// </summary>
        public IDisposalScope DisposalScope
        {
            get { return _disposalScope; }
            set { _disposalScope = value; }
        }

        /// <summary>
        /// The scope that the request originated in
        /// </summary>
        public IInjectionScope RequestingScope
        {
            get { return _requestingScope; }
            set { _requestingScope = value; }
        }

        /// <summary>
        /// The target information for the current injection
        /// </summary>
        public IInjectionTargetInfo TargetInfo
        {
            get { return _targetInfo; }
            set { _targetInfo = value; }
        }

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

            if (_extraData != null)
            {
                _extraData.TryGetValue(dataName, out returnValue);
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
            if (_extraData == null)
            {
                _extraData = ImmutableHashTree<string, object>.Empty;
            }

            _extraData = _extraData.Add(dataName, newValue);
        }

        /// <summary>
        /// Locate an export by type
        /// </summary>
        /// <returns></returns>
        public object Locate<T>()
        {
            if (_exportsByType != null)
            {
                ExportActivationDelegate activationDelegate;

                if (_exportsByType.TryGetValue(typeof(T), out activationDelegate))
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
            if (_exportsByType != null)
            {
                ExportActivationDelegate activationDelegate;

                if (_exportsByType.TryGetValue(type, out activationDelegate))
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
            if (_exportsByName != null)
            {
                ExportActivationDelegate activationDelegate;

                if (_exportsByName.TryGetValue(name.ToLowerInvariant(), out activationDelegate))
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
            if (_exportsByType == null)
            {
                _exportsByType = ImmutableHashTree<Type, ExportActivationDelegate>.Empty;
            }

            _exportsByType = _exportsByType.Add(exportType, activationDelegate);
        }

        /// <summary>
        /// Register an export by name for this injection context
        /// </summary>
        /// <param name="name"></param>
        /// <param name="activationDelegate"></param>
        public void Export(string name, ExportActivationDelegate activationDelegate)
        {
            name = name.ToLowerInvariant();

            if (_exportsByName == null)
            {
                _exportsByName = ImmutableHashTree<string, ExportActivationDelegate>.Empty;
            }

            _exportsByName = _exportsByName.Add(name, activationDelegate);
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
            if (_resolveDepth > MaxResolveDepth)
            {
                if (_targetInfo != null)
                {
                    throw new CircularDependencyDetectedException(_targetInfo.LocateName, _targetInfo.LocateType, this);
                }

                throw new CircularDependencyDetectedException(null, (Type)null, this);
            }

            if (_resolveDepth >= _currentInjectionInfo.Length)
            {
                var temp = new CurrentInjectionInfo[_currentInjectionInfo.Length * 2];

                _currentInjectionInfo.CopyTo(temp, 0);

                _currentInjectionInfo = temp;
            }

            _currentInjectionInfo[_resolveDepth] = new CurrentInjectionInfo(typeof(T), exportStrategy, _targetInfo);

            _resolveDepth++;
        }

        /// <summary>
        /// Pop the current export strategy off the stack
        /// </summary>
        public void PopCurrentInjectionInfo()
        {
            _resolveDepth--;
        }

        /// <summary>
        /// Injection info all the way up the stack
        /// </summary>
        /// <returns></returns>
        public ImmutableArray<CurrentInjectionInfo> GetInjectionStack()
        {
            return ImmutableArray.From(_currentInjectionInfo, _resolveDepth);
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

        /// <summary>
        /// Creates a new context, both disposal scope and scope must be provided
        /// </summary>
        /// <param name="disposalScope"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static IInjectionContext DefaultCreateContext(IDisposalScope disposalScope, IInjectionScope scope)
        {    
            return new InjectionContext(disposalScope, scope);
        }
    }
}