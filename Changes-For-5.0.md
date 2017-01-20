Changes for 5.0
===============

5.0 is a major rewrite/refactoring of the Grace code base. The main objective of the rewrite was to change Grace's dependency location paradigm from request time to pre-calculated at first request (i.e. dependency is calculated once and never again). This was done mainly for performance reason but also to keep dependencies from changing while executing (it's still possible it just not the default). 

**New Features**
* [Decorators](https://en.wikipedia.org/wiki/Decorator_pattern) - Register classes to be used as decorators when locating exports

* Custom Wrappers - Register classes to be used as wrappers similar to Meta&lt;T&gt;

* Override IEnumerable&lt;T&gt; creation - customize the creation of IEnumerable&lt;T&gt; 

* ExportFactory - allows the developer to export a method with dependencies injected into arguments

* Export Method - you can now export a method to be used as dependencies for other registrations

* Performance - Grace now performs as fast or faster than all other containers in a number of usecases. 

**Changes**

* Metadata key changed from string to object, to allow for use of typeof() as metadata key

* Export Type Set changed Select to Where

* IExportStrategy was replaced with ICompiledExportStrategy

* ILifestyle was replaced with ICompiledLifestyle

* InjectionContext broken into two different types, StaticInjectionContext (information known at configuration time) and InjectionContext per object graph context object (not created unless needed)

* IExportLocator -> IExportLocatorScope taking over more functionality from IInjectionScope

* ILocatorService was introduced it defines all Locate methods without the complexity of IExportLocatorScope, it's recommend you inject this instead of IExportLocatorScope

* BeginLifetimeScope creates IExportLocatorScope instead of IInjectionScope

* IInjectionScope won't inject by default, more than likely you want IExportLocatorScope

* Named exports will be implemented as an extension rather than first class method on the registration block

* Simple exports will be implemented as an extension rather than first class method on the registration block

* Conditions are tested once at expression creation time (first time located) rather than every time an export is located, runtime conditions can be implemented as ExportInstance or ExportFactory

* When____ condition methods moved to When.______ this was to clean up the interface

* Func&lt;Type,Object&gt; is not supported out of the box. It's easy to add by hand and very few projects need support for it.

* WithKey and AsKeyed&lt;TInterface,TKey&gt;() have been consolidated into AsKeyed&gt;TInterface&lt;(object key)

* WCF support no longer uses a custom disposal scope provider rather a lifetime scope is created for each request
