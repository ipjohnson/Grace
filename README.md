Meet Grace
=====

Grace is a feature rich Dependency Injection Container designed with ease of use and performance in mind. 

```csharp 
   using Grace.DependencyInjection;

   DependencyInjectionContainer container = new DependencyInjectionContainer();

   container.Configure(c => c.Export<BasicService>().As<IBasicService>());

   IBasicService basicService = container.Locate<IBasicService>();
```

It's as easy as create, configure, and locate.

Getting Grace
=====

A NuGet package supporting .Net 4.5, Windows Phone 8 and WinRT can be found [here](http://www.nuget.org/packages/Grace/)

Visual Studio Templates for creating new MVC, WCF, and Nancy applications can be found [here](http://visualstudiogallery.msdn.microsoft.com/db6d9cb6-bb84-4c98-82b7-2edc4388cdef)




