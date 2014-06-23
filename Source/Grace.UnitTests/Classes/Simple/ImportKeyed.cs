using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
    public interface IImportKeyed
    {
        ISimpleObject SimpleObject { get; }
    }

    public class ImportKeyedA : IImportKeyed
    {
        public ImportKeyedA(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public class ImportKeyedB : IImportKeyed
    {
        public ImportKeyedB(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public class ImportKeyedC : IImportKeyed
    {
        public ImportKeyedC(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public class ImportKeyedD : IImportKeyed
    {
        public ImportKeyedD(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public class ImportKeyedE : IImportKeyed
    {
        public ImportKeyedE(ISimpleObject simpleObject)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }
}
