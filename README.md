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

[NuGet](http://www.nuget.org/packages/Grace/) package supports .Net 4.5, Windows Phone 8 and WinRT.

[Visual Studio Templates](http://visualstudiogallery.msdn.microsoft.com/db6d9cb6-bb84-4c98-82b7-2edc4388cdef) are avaliable for creating new MVC, WCF, and Nancy applications as well


Documented Grace
====

[Getting Started](https://github.com/ipjohnson/Grace/wiki/Getting-Started)  - A getting started guide for Grace as well as documentation for more complex topics 

[API Documentation](http://www.nudoq.org/#!/Packages/Grace/Grace/DependencyInjectionContainer) - API document is hosted on NuDoq

Features in Grace
=====

* Fluent interface or Attributes for configuration allowing for maximum flexibility
* Supports child containers
* Contextual binding support (similar to NInject)
* IDisposable objects created by the container will be tracked and disposed by the container unless configured not to.
* Performance characteristics that make it faster than most of the containers out there inluding Autofac, Castle Windsor, MEF, NInject, StructureMap and Unity. 
* Supports special types
   * IEnumerable<T> - supports resolving collections as IEnumerable as well as most other types of collections (List<T>, ReadOnlyCollection<T>, T[])
   * Func<T> - supports resolving Func<T> automatically
   * Lazy<T> - when resolved a Lazy<T> will be created that resolves T from the scope it was created in
   * Owned<T> - object resolved within a Owned<T> will have their disposal lifecycle tied to the Owned<T> (similar to autofac)
   * Meta<T> - objects resolved within a Meta<T> are resolved along with their metadata


Builds of Grace
=======

Grace has public builds at [CodeBetter](http://teamcity.codebetter.com/project.html?projectId=project416)

Nightly Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1138"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:bt1138/statusIcon"/></a>

CI Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1137"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:bt1137/statusIcon"/></a>


