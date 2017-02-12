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

[NuGet](http://www.nuget.org/packages/Grace/) package supports .Net 4.5, .Net Standard 1.0, and PCL Profile 259.

[Getting Started](https://github.com/ipjohnson/Grace/wiki/Getting-Started)  - A getting started guide for Grace as well as documentation for more complex topics 

Features in Grace
=====

* Fluent interface or Attributes for configuration allowing for maximum flexibility
* Supports child containers and light weight lifetime scopes
* Contextual binding support (similar to NInject)
* IDisposable objects created by the container will be tracked and disposed by the container unless configured otherwise.
* Performance characteristics that make it one of the fastest containers available. ([Benchmarks](http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison))
* Supports special types
   * IEnumerable&lt;T&gt; - supports resolving collections as IEnumerable&lt;T&gt; as well as most other types of collections List&lt;T&gt;, ReadOnlyCollection&lt;T&gt;, T[] and any collection that implements ICollection&lt;T&gt;
   * Func&lt;T&gt; - supports resolving Func<T&gt; automatically
   * Lazy&lt;T&gt; - when resolved a Lazy&lt;T&gt; will be created that resolves T from the scope it was created in
   * Owned&lt;T&gt; - object resolved within a Owned&lt;T&gt; will have their disposal lifecycle tied to the Owned&lt;T&gt; (similar to autofac)
   * Meta&lt;T&gt; - objects resolved within a Meta&lt;T&gt; are resolved along with their metadata
   * Custom Delegates - any delegate that returns a type can be automatically resolved. 
* Many LifeStyles supported including Singleton, SingletonPerScope, SingletonPerRequest (MVC4, MVC5 & WCF packages), SingletonPerObjectGraph, SingletonPerAncestor&lt;T&gt;, and WeakSingleton. If none of the provided life styles meet your need you can always implement your own ICompiledLifeStyle class.



Builds of Grace
=======

Grace has public builds at AppVeyor

[![Build status](https://ci.appveyor.com/api/projects/status/rpgdoopau4hh1ltn?svg=true)](https://ci.appveyor.com/project/ipjohnson/grace) [![Coverage Status](https://coveralls.io/repos/github/ipjohnson/Grace/badge.svg)](https://coveralls.io/github/ipjohnson/Grace)
