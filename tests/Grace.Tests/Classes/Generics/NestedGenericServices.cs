using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.Tests.Classes.Generics
{
    public interface IGenericServiceA<T>
    {
        string GetValue(T value);
    }

    public class GenericServiceA<T> : IGenericServiceA<T>
    {
        private GenericServiceB<T> _genericServiceB;

        public GenericServiceA(GenericServiceB<T> genericServiceB)
        {
            _genericServiceB = genericServiceB;
        }

        public string GetValue(T value)
        {
            return _genericServiceB.GetValue(value);
        }
    }

    public interface IGenericServiceB<T>
    {
        string GetValue(T value);
    }

    public class GenericServiceB<T> : IGenericServiceB<T>
    {
        public string GetValue(T value)
        {
            return value.ToString();
        }
    }
}
