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

[Visual Studio Templates](http://visualstudiogallery.msdn.microsoft.com/db6d9cb6-bb84-4c98-82b7-2edc4388cdef) are avaliable for creating new MVC 5, WCF, and Nancy applications


Documented Grace
====

[Getting Started](https://github.com/ipjohnson/Grace/wiki/Getting-Started)  - A getting started guide for Grace as well as documentation for more complex topics 

[API Documentation](http://www.nudoq.org/#!/Packages/Grace/Grace/DependencyInjectionContainer) - API document is hosted on NuDoq

Features in Grace
=====

* Fluent interface or Attributes for configuration allowing for maximum flexibility
* Supports child containers and light weight lifetime scopes
* Contextual binding support (similar to NInject)
* IDisposable objects created by the container will be tracked and disposed by the container unless configured otherwise.
* Performance characteristics that make it faster than most of the containers out there inluding Autofac, Castle Windsor, MEF, NInject, StructureMap and Unity. ([Benchmarks](http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison))
* Supports special types
   * IEnumerable&lt;T&gt; - supports resolving collections as IEnumerable&lt;T&gt; as well as most other types of collections List&lt;T&gt;, ReadOnlyCollection&lt;T&gt;, T[] and any collection that implements ICollection&lt;T&gt;
   * Func&lt;T&gt; - supports resolving Func<T&gt; automatically
   * Lazy&lt;T&gt; - when resolved a Lazy&lt;T&gt; will be created that resolves T from the scope it was created in
   * Owned&lt;T&gt; - object resolved within a Owned&lt;T&gt; will have their disposal lifecycle tied to the Owned&lt;T&gt; (similar to autofac)
   * Meta&lt;T&gt; - objects resolved within a Meta&lt;T&gt; are resolved along with their metadata
   * Custom Delegates - any delegate that returns a type can be automatically resolved. 
* Many LifeStyles supported including Singleton, SingletonPerScope, SingletonPerRequest (MVC & WCF out of the box), SingletonPerAncestor&lt;T&gt;, WeakSingleton and more. If none of the provided life styles meet your need you can always implement your own ILifeStyle class.



Builds of Grace
=======

Grace has public builds at [CodeBetter](http://teamcity.codebetter.com/project.html?projectId=project416)

Nightly Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=Grace_GraceNightly"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:Grace_GraceNightly/statusIcon"/></a>

CI Build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1137"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:bt1137/statusIcon"/></a>

Metrics by Ohloh:

[
![Ohloh project report for Grace IoC](https://www.ohloh.net/p/Grace-IoC/widgets/project_thin_badge.gif)
](http://www.ohloh.net/p/Grace-IoC?ref=sample)

<div style="background: #00578e url('http://www.jetbrains.com/img/banners/Codebetter300x250.png') no-repeat 0 50%; margin:0;padding:0;text-decoration:none;text-indent:0;letter-spacing:-0.001em; width:300px; height:250px"> <a href="http://www.jetbrains.com/youtrack" title="YouTrack by JetBrains" style="margin: 52px 0 0 58px;padding: 0; float: left;font-size: 14px; background-image:none;border:0;color: #acc4f9; font-family: trebuchet ms,arial,sans-serif;font-weight: normal;text-align:left;">keyboard-centric bug tracker</a> <a href="http://www.jetbrains.com/teamcity" title="TeamCity by JetBrains" style="margin:0 0 0 58px;padding:122px 0 0 0;font-size:14px; background-image:none;border:0;display:block; color: #acc4f9; font-family: trebuchet ms,arial,sans-serif;font-weight: normal;text-align:left;">continuous integration server</a> </div>
