using System;

namespace Grace.Tests.Classes.Simple
{
    public interface ITypeHolderService
    {
        Type InjectedType { get; }
    }
    public class TypeHolderService : ITypeHolderService
    {
        public TypeHolderService(Type type)
        {
            InjectedType = type;
        }
        public Type InjectedType { get; }
    }

    public interface ITypeHolderDependency
    {
        ITypeHolderService Holder { get; }
    }

    public class TypeHolderDependency : ITypeHolderDependency
    {
        public TypeHolderDependency(ITypeHolderService service)
        {
            Holder = service;
        }

        public ITypeHolderService Holder { get; }

    }
}
