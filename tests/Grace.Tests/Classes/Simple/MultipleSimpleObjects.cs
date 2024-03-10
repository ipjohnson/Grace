using System;

namespace Grace.Tests.Classes.Simple
{
    public class SimpleFilterAttribute : Attribute
    {

    }

    public class ImportSingleSimpleObject
    {
        public ImportSingleSimpleObject(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; }
    }

    public class ImportMultipleSimpleObjects
    {
        public ImportMultipleSimpleObjects(ISimpleObject[] simpleObjects)
        {
            SimpleObjects = simpleObjects;
        }

        public ISimpleObject[] SimpleObjects { get; }
    }

    [SimpleFilter]
    public interface ISimpleObject
    {
        string TestString { get; }
    }

    [SimpleFilter]
    public class SimpleObjectA : ISimpleObject
    {
        public string TestString => "A";
    }

    public class SimpleObjectB : ISimpleObject
    {
        public string TestString => "B";
    }

    [SimpleFilter]
    public class SimpleObjectC : ISimpleObject
    {
        public string TestString => "C";
    }

    public class SimpleObjectD : ISimpleObject
    {
        public string TestString => "D";
    }

    [SimpleFilter]
    public class SimpleObjectE : ISimpleObject
    {
        public string TestString => "E";
    }
}
