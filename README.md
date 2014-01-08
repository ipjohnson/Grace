Meet Grace
=====

Grace is a feature rich Dependency Injection Container designed with performance and ease of use in mind. 

`   
   using Grace.DependencyInjection;

   DependencyInjectionContainer container = new DependencyInjectionContainer();

   container.Configure(c => c.Export<BasicService>().As<IBasicService>());

   IBasicService basicService = container.Locate<IBasicService>();`


It's as easy as that. 

Getting Started
=====

You have the choice of using starting with the NuGet package