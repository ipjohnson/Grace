namespace Grace.DependencyInjection.Named
{
    /// <summary>
    /// Extension class for named exports
    /// </summary>
    public static class ExportNamedExtensions
    {
        //public static object Locate(this IExportLocatorScope scope, string exportName)
        //{
        //    var current = scope;
        //    INamedResolutionService service = null;

        //    while (current != null)
        //    {
        //        if (current is IInjectionScope)
        //        {
        //            service = (INamedResolutionService)current.GetExtraData(typeof(INamedResolutionService));

        //            if (service != null)
        //            {
        //                object returnValue = service.GetNamed(scope, exportName);

        //                if (returnValue != null)
        //                {
        //                    return returnValue;
        //                }
        //            }
        //        }

        //        current = current.Parent;
        //    }

        //    throw new Exception($"Could not locate named export ${exportName}");
        //}

        //public static IFluentExportStrategyConfiguration AsName(this IFluentExportStrategyConfiguration configuration, string name)
        //{
        //    var activationType = configuration.Strategy.ActivationType;

        //    configuration.As(activationType);

        //    INamedResolutionService service = (INamedResolutionService)configuration.Strategy.InjectionScope.GetExtraData(typeof(INamedResolutionService));

        //    if (service == null)
        //    {
        //        service = new NamedResolutionService(configuration.Strategy.InjectionScope);

        //        service = (INamedResolutionService)configuration.Strategy.InjectionScope.SetExtraData(typeof(INamedResolutionService), service, false);
        //    }

        //    service.ExportAs(configuration.Strategy, name);

        //    return configuration;
        //}
    }
}
