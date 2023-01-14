namespace Grace.Tests.Classes.Simple
{


    #region ISomePropertyService

    public interface ISomePropertyService
    {
        object SomeProperty { get; }
    }

    public class StringArgSomePropertyService : ISomePropertyService
    {
        public StringArgSomePropertyService(string testString)
        {
            SomeProperty = testString;
        }

        public object SomeProperty { get; }
    }

    public class ReferenceArgSomePropertyService : ISomePropertyService
    {
        public ReferenceArgSomePropertyService(IBasicService basicService)
        {
            SomeProperty = basicService;
        }

        public object SomeProperty { get; }
    }

    #endregion

    #region IArrayOfObjectsPropertyService

    public interface IArrayOfObjectsPropertyService
    {
        object[] Parameters { get; }
    }

    public class OneArgStringParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService Activate(string stringArg);

        public OneArgStringParameterService(string stringArg)
        {
            Parameters = new object[] { stringArg };
        }

        public object[] Parameters { get; }
    }

    public class OneArgRefParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService ActivateWithBasicService(IBasicService basicService);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicService();

        public OneArgRefParameterService(IBasicService basicService)
        {
            Parameters = new object[] { basicService };
        }

        public object[] Parameters { get; }
    }

    public class TwoArgParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService ActivateWithBasicService(
            string stringArg,
            IBasicService basicService);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicService(
            string stringArg);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicServiceAndOutOfOrder(
            string stringArg);

        public TwoArgParameterService(string stringArg, IBasicService basicService)
        {
            Parameters = new object[] { stringArg, basicService };
        }

        public object[] Parameters { get; }
    }

    public class ThreeArgParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService ActivateWithBasicService(
            string stringArg,
            int intArg,
            IBasicService basicService);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicService(
            string stringArg,
            int intArg);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicServiceAndOutOfOrder(
            int intArg,
            string stringArg);

        public ThreeArgParameterService(string stringArg, int intArg, IBasicService basicService)
        {
            Parameters = new object[] { stringArg, intArg, basicService };
        }

        public object[] Parameters { get; }
    }

    public class FourArgParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService ActivateWithBasicService(
            string stringArg,
            int intArg,
            double doubleArg,
            IBasicService basicService);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicService(
            string stringArg,
            int intArg,
            double doubleArg);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicServiceAndOutOfOrder(
            double doubleArg,
            int intArg,
            string stringArg);

        public FourArgParameterService(string stringArg, int intArg, double doubleArg, IBasicService basicService)
        {
            Parameters = new object[] { stringArg, intArg, doubleArg, basicService };
        }

        public object[] Parameters { get; }
    }

    public class FiveArgParameterService : IArrayOfObjectsPropertyService
    {
        public delegate IArrayOfObjectsPropertyService ActivateWithBasicService(
            string stringArg,
            int intArg,
            double doubleArg,
            decimal decimalArg,
            IBasicService basicService);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicService(
            string stringArg,
            int intArg,
            double doubleArg,
            decimal decimalArg);

        public delegate IArrayOfObjectsPropertyService ActivateWithOutBasicServiceAndOutOfOrder(
            decimal decimalArg,
            string stringArg,
            double doubleArg,
            int intArg);

        public FiveArgParameterService(string stringArg,
            int intArg,
            double doubleArg,
            decimal decimalArg,
            IBasicService basicService)
        {
            Parameters = new object[] { stringArg, intArg, doubleArg, decimalArg, basicService };
        }

        public object[] Parameters { get; }
    }

    #endregion
}
