using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.Tests.Classes.Attributes
{
  public class AdaptedImportAttribute : Attribute
  {
    static AdaptedImportAttribute()
    {
      // Note that XUnit 3.0 will run tests in parallel by default.
      // When upgrading, this initializer should be moved into the new XUnit 3.0 entry point,
      // to avoid concurrency issues.
      ImportAttributeInfo.RegisterImportAttributeAdapter<AdaptedImportAttribute>(
        (attr, type, name) => new ImportAttributeInfo 
        { 
          ImportKey = ((AdaptedImportAttribute)attr).Key,
          IsRequired = true,
        });
    }

    public string Key { get; set; }
  }
}