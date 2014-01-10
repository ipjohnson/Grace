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


Documented Grace
====

[Getting Started](https://github.com/ipjohnson/Grace/wiki/Getting-Started)  - A getting started guide for Grace as well as documentation for more complex topics (this is current being ported over from codeplex so bear with me)

[API Documentation](http://www.nudoq.org/#!/Packages/Grace/Grace/DependencyInjectionContainer) - API document is hosted on NuDoq


Development Builds
=======

Grace has public builds at [CodeBetter](http://teamcity.codebetter.com/project.html?projectId=project416)

Nightly Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1138"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:bt1138/statusIcon"/></a>

CI Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1137"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:bt1137/statusIcon"/></a>