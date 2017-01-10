using Grace.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using Grace.AspNetCore.MVC.Inspector;

namespace Grace.AspNetCore.MVC
{
    /// <summary>
    /// Configuration class for MVC extension
    /// </summary>
    public class GraceMVCConfiguration
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GraceMVCConfiguration()
        {
            UseControllerActivator = true;
            UseViewActivator = true;
            SupportHttpInfoInjection = true;
        }

        /// <summary>
        /// Use custom controller activator
        /// </summary>
        public bool UseControllerActivator { get; set; }

        /// <summary>
        /// Use custom view activator
        /// </summary>
        public bool UseViewActivator { get; set; }
        
        /// <summary>
        /// Support injecting http info as dependency
        /// </summary>
        public bool SupportHttpInfoInjection { get; set; }        
    }

    /// <summary>
    /// C# extension class
    /// </summary>
    public static class MVCConfigurationExtensions
    {
        /// <summary>
        /// Setup MVC extension for Grace (Controller activtor, View activator, MVC data as dependency)
        /// </summary>
        /// <param name="scope">injection scope to setup </param>
        /// <param name="configure">configuration action</param>
        public static void SetupMvc(this IInjectionScope scope, Action<GraceMVCConfiguration> configure = null)
        {
            var configuration = new GraceMVCConfiguration();

            configure?.Invoke(configuration);
            
            scope.Configure(c =>
            {
                if (configuration.SupportHttpInfoInjection)
                {
                    c.AddInspector(new BindingSourceAttributePropertyInspector());
                    c.AddInjectionValueProvider(new BindingSourceMetadataValueProvider());
                }
                
                if (configuration.UseControllerActivator)
                {
                    c.Export<GraceControllerActivator>().As<IControllerActivator>().WithPriority(10).Lifestyle.Singleton();
                }

                if(configuration.UseViewActivator)
                {
                    c.Export<GraceViewActivator>().As<IViewComponentActivator>().WithPriority(10).Lifestyle.Singleton();
                }                
            });
        }
    }
}
