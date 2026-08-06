using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.Utilities;

namespace Grace.Factory.Impl
{
    /// <summary>
    /// Dynamic proxy type builder
    /// </summary>
    public class DynamicTypeBuilder
    {
        private static readonly object BuilderLock = new object();
        private static ModuleBuilder _moduleBuilder;
        private static int _proxyCount = 0;
        private static MethodInfo CloneMethod = typeof(IInjectionContext).GetRuntimeMethod(nameof(IInjectionContext.Clone), Type.EmptyTypes);
        private static MethodInfo DelegateInvoke = typeof(ActivationStrategyDelegate)
            .GetRuntimeMethod(
                nameof(ActivationStrategyDelegate.Invoke),
                new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext), typeof(object) });
        private static MethodInfo SetExtraDataMethod = typeof(IExtraDataContainer)
            .GetRuntimeMethod(
                nameof(IExtraDataContainer.SetExtraData), 
                new[] { typeof(object), typeof(object), typeof(bool) });

        /// <summary>
        /// Default constructor
        /// </summary>
        public DynamicTypeBuilder()
        {
            SetupAssembly();
        }
        
        public class DelegateInfo
        {
            public DelegateInfo(MethodInfo method, List<DelegateParameterInfo> parameterInfos, FieldBuilder activation)
            {
                Method = method;
                ParameterInfos = parameterInfos;
                Activation = activation;
            }

            public MethodInfo Method { get; }

            public List<DelegateParameterInfo> ParameterInfos { get; }

            public FieldBuilder Activation { get; }
        }

        public class DelegateParameterInfo
        {
            public DelegateParameterInfo(ParameterInfo parameterInfo)
            {
                ParameterInfo = parameterInfo;
                UniqueId = UniqueStringId.Generate();
            }

            public string UniqueId { get; }

            public ParameterInfo ParameterInfo { get; }
        }

        /// <summary>
        /// Create new proxy type that implements the interface
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <param name="methods">delegates that needed for constructor</param>
        public Type CreateType(Type interfaceType, out List<DelegateInfo> methods)
        {
            lock (BuilderLock)
            {
                _proxyCount++;

                var proxyBuilder = _moduleBuilder.DefineType(interfaceType.Name + "Proxy" + _proxyCount,
                    TypeAttributes.Class | TypeAttributes.Public);

                proxyBuilder.AddInterfaceImplementation(interfaceType);

                var scopeField = proxyBuilder.DefineField("_scope", typeof(IExportLocatorScope),
                    FieldAttributes.Private);

                var disposalScopeField = proxyBuilder.DefineField("_disposalScope", typeof(IDisposalScope),
                    FieldAttributes.Private);

                var contextField = proxyBuilder.DefineField("_context", typeof(IInjectionContext),
                    FieldAttributes.Private);

                var keyField = proxyBuilder.DefineField("_key", typeof(object),
                    FieldAttributes.Private);

                int methodCount = 1;

                methods = new List<DelegateInfo>();

                foreach (var method in interfaceType.GetRuntimeMethods())
                {
                    methods.Add(CreateMethod(method, proxyBuilder, methodCount, scopeField, disposalScopeField, contextField, keyField));

                    methodCount++;
                }

                CreateConstructor(proxyBuilder, methods, scopeField, disposalScopeField, contextField, keyField);

                return proxyBuilder.CreateTypeInfo().AsType();
            }
        }

        private void CreateConstructor(
            TypeBuilder proxyBuilder, 
            List<DelegateInfo> methods,
            FieldBuilder scopeField, 
            FieldBuilder disposalScopeField, 
            FieldBuilder contextField, 
            FieldBuilder keyField)
        {
            var constructorParameterTypes = new List<Type> 
            { 
                typeof(IExportLocatorScope), 
                typeof(IDisposalScope), 
                typeof(IInjectionContext),
                typeof(object),
            };

            foreach (var _ in methods)
            {
                constructorParameterTypes.Add(typeof(ActivationStrategyDelegate));
            }

            var proxyConstructor = proxyBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, constructorParameterTypes.ToArray());

            for (int i = 1; i <= constructorParameterTypes.Count; i++)
            {
                proxyConstructor.DefineParameter(i, ParameterAttributes.None, "arg" + i);
            }

            var proxyConstructorILGenerator = proxyConstructor.GetILGenerator();

            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_1);
            proxyConstructorILGenerator.Emit(OpCodes.Stfld, scopeField);

            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_2);
            proxyConstructorILGenerator.Emit(OpCodes.Stfld, disposalScopeField);

            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_3);
            proxyConstructorILGenerator.Emit(OpCodes.Stfld, contextField);

            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_S, (byte)4);
            proxyConstructorILGenerator.Emit(OpCodes.Stfld, keyField);

            byte argIndex = 5;
            foreach (var method in methods)
            {
                proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
                proxyConstructorILGenerator.Emit(OpCodes.Ldarg_S, argIndex++);
                proxyConstructorILGenerator.Emit(OpCodes.Stfld, method.Activation);
            }

            proxyConstructorILGenerator.Emit(OpCodes.Ret);
        }

        private DelegateInfo CreateMethod(
            MethodInfo method, 
            TypeBuilder proxyBuilder, 
            int methodCount, 
            FieldBuilder scopeField, 
            FieldBuilder disposalScopeField, 
            FieldBuilder contextField,
            FieldBuilder keyField)
        {
            var parameters = method.GetParameters();

            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot;

            var methodBuilder = proxyBuilder.DefineMethod(method.Name, methodAttributes, method.ReturnType, parameters.Select(x => x.ParameterType).ToArray());

            var ilGenerator = methodBuilder.GetILGenerator();

            var activation = proxyBuilder.DefineField("_activationMethod" + methodCount, typeof(ActivationStrategyDelegate),
                FieldAttributes.Private);          
            
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, activation);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, scopeField);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, disposalScopeField);

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, contextField);
            ilGenerator.Emit(OpCodes.Callvirt, CloneMethod);

            var parameterInfos = new List<DelegateParameterInfo>();

            foreach (var parameter in parameters)
            {
                ilGenerator.Emit(OpCodes.Dup);

                var delegateParameterInfo = new DelegateParameterInfo(parameter);

                parameterInfos.Add(delegateParameterInfo);

                ilGenerator.Emit(OpCodes.Ldstr, delegateParameterInfo.UniqueId);

                switch (parameter.Position)
                {
                    case 0:
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        ilGenerator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        ilGenerator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        ilGenerator.Emit(OpCodes.Ldarg_S, (byte)(parameter.Position + 1));
                        break;
                }

                ilGenerator.Emit(OpCodes.Box, parameter.ParameterType);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);

                ilGenerator.Emit(OpCodes.Callvirt, SetExtraDataMethod);

                ilGenerator.Emit(OpCodes.Pop);
            }

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, keyField);

            ilGenerator.Emit(OpCodes.Callvirt, DelegateInvoke);

            if (method.ReturnType != null && method.ReturnType != typeof(void))
            {
                ilGenerator.Emit(
                    method.ReturnType.GetTypeInfo().IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass,
                    method.ReturnType);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Pop);
            }

            ilGenerator.Emit(OpCodes.Ret);

            proxyBuilder.DefineMethodOverride(methodBuilder, method);

            return new DelegateInfo(method, parameterInfos, activation);
        }

        private void SetupAssembly()
        {
            if (_moduleBuilder == null)
            {
                lock (BuilderLock)
                {
                    if (_moduleBuilder == null)
                    {
                        var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);

                        _moduleBuilder = dynamicAssembly.DefineDynamicModule("DynamicProxyTypes");
                    }
                }
            }
        }
    }
}
