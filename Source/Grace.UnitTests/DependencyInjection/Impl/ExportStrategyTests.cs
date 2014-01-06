using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class ExportStrategyTests
	{
		[Fact]
		public void BasicTestOFAll()
		{
			FauxInjectionScope scope = new FauxInjectionScope();

			foreach (IExportStrategy exportStrategy in Strategies())
			{
				IConfigurableExportStrategy configurableExport = exportStrategy as IConfigurableExportStrategy;

				if (configurableExport != null)
				{
					configurableExport.OwningScope = scope;

					configurableExport.Initialize();
				}

				string activationName = exportStrategy.ActivationName;
				ExportEnvironment environment = exportStrategy.Environment;
				IEnumerable<string> exportNames = exportStrategy.ExportNames;

				exportNames.Count();

				bool externally = exportStrategy.ExternallyOwned;
				bool hasConditions = exportStrategy.HasConditions;
				object key = exportStrategy.Key;
				ILifestyle container = exportStrategy.Lifestyle;
				IInjectionScope owningScope = exportStrategy.OwningScope;
				IReadOnlyDictionary<string, object> metadata = exportStrategy.Metadata;

				exportStrategy.MeetsCondition(new FauxInjectionContext());

				FauxInjectionScope fauxScope = new FauxInjectionScope();

				exportStrategy.Activate(fauxScope, new FauxInjectionContext { RequestingScope = fauxScope }, null);

				exportStrategy.Dispose();
			}
		}

		private IEnumerable<IExportStrategy> Strategies()
		{
			yield return new CompiledInstanceExportStrategy(typeof(BasicService));
			yield return new CompiledFuncExportStrategy<BasicService>((scope, context) => new BasicService());
			yield return new AttributeExportStrategy(typeof(BasicService), new Attribute[0]);
			yield return new ArrayExportStrategy<IBasicService>();
			yield return new ListExportStrategy<IBasicService>();
			yield return new ReadOnlyCollectionExportStrategy<IBasicService>();
			yield return new FuncExportStrategy<IBasicService>();
			yield return new LazyExportStrategy<IBasicService>();
			yield return new GenericFuncExportStrategy<IImportMethodService, IBasicService>();
			yield return new GenericFuncExportStrategy<IImportMethodService, IImportMethodService, IBasicService>();
			yield return
				new GenericFuncExportStrategy<IImportMethodService, IImportMethodService, IImportMethodService, IBasicService>();
			yield return
				new GenericFuncExportStrategy
					<IImportMethodService, IImportMethodService, IImportMethodService, IImportMethodService, IBasicService>();
			yield return
				new GenericFuncExportStrategy
					<IImportMethodService, IImportMethodService, IImportMethodService, IImportMethodService, IImportMethodService,
						IBasicService>();
		}
	}
}