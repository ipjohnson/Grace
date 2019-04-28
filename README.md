# Grace

<img src="https://raw.githubusercontent.com/ipjohnson/Grace/master/img/logo.svg?sanitize=true" width="76">

[![Build status](https://ci.appveyor.com/api/projects/status/rpgdoopau4hh1ltn?svg=true)](https://ci.appveyor.com/project/ipjohnson/grace) [![Coverage Status](https://coveralls.io/repos/github/ipjohnson/Grace/badge.svg)](https://coveralls.io/github/ipjohnson/Grace) [![Stack Overflow](https://img.shields.io/badge/StackOverflow-QnA-green.svg)](http://stackoverflow.com/questions/tagged/grace-ioc)

Grace is a feature-rich dependency injection container designed with ease of use and performance in mind. 

```csharp 
using Grace.DependencyInjection;

var container = new DependencyInjectionContainer();

container.Configure(c => c.Export<BasicService>().As<IBasicService>());

var basicService = container.Locate<IBasicService>();
```

It's as easy as create, configure, and locate.

Getting Grace
=====

[NuGet](http://www.nuget.org/packages/Grace/) package supports .Net 4.5 and .Net Standard 1.0.

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
   * Owned&lt;T&gt; - object resolved within a Owned&lt;T&gt; will have their disposal lifecycle tied to the Owned&lt;T&gt; (similar to Autofac)
   * Meta&lt;T&gt; - objects resolved within a Meta&lt;T&gt; are resolved along with their metadata
   * Custom Delegates - any delegate that returns a type can be automatically resolved. 
   * Custom interface factories with Grace.Factory
* Many LifeStyles supported including Singleton, SingletonPerScope, SingletonPerRequest (MVC4, MVC5 & WCF packages), SingletonPerObjectGraph, SingletonPerAncestor&lt;T&gt;, and WeakSingleton. If none of the provided life styles meet your need you can always implement your own ICompiledLifeStyle class.
* Built in support for the decorator pattern
* Support for custom wrappers (Func&lt;T&gt; and Meta&lt;T&gt; are examples of built-in wrappers)
* [ASP.Net Core](https://github.com/ipjohnson/Grace.DependencyInjection.Extensions) supports
* [Source Link](https://github.com/dotnet/sourcelink) support enabled

Special thanks to @silkfire for the logo
