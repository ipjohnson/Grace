using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// Imports the currently located key.
    /// Usually used on keyed services registered as ImportKey.Any.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ImportKeyAttribute : Attribute, IImportAttribute
    {
        ImportAttributeInfo IImportAttribute.ProvideImportInfo(Type attributedType, string attributedName)
        {
            return new ImportAttributeInfo
            {
                ImportKey = ImportKey.Key,
                IsRequired = true,
            };
        }
    }
}