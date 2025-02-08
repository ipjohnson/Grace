using System;
using Grace.DependencyInjection.Attributes;

namespace Grace.Tests.Classes.Simple
{
    public class ImportKeyService : IComparable<ImportKeyService>
    {
        public ImportKeyService()
        { }

        public ImportKeyService(object key)
        { 
            ObjectKey = key;
        }

        public ImportKeyService(object key, int extraParameter)
        {
            ObjectKey = key;
            IntKey = extraParameter;
        }

        public void ImportMethod(object key)
        { 
            ObjectKey = key;
        }

        public int CompareTo(ImportKeyService other) => IntKey.CompareTo(other.IntKey);

        public object ObjectKey { get; set; }
        public string StringKey { get; set; }
        public int IntKey { get; set; }
    }

    public class ImportKeyServiceWrapper
    {
        public ImportKeyServiceWrapper(ImportKeyService service)
        {
            Service = service;
        }

        public ImportKeyService Service { get; }

        public object ObjectKey { get; set; }
    }

    class ServiceWithKeyWrapper<T>
    {
        public ServiceWithKeyWrapper([Import(Key = "Keyed")] T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}