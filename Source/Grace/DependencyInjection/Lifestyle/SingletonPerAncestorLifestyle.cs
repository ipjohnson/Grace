using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Singleton per ancestor type
	/// </summary>
	public class SingletonPerAncestorLifestyle : ILifestyle
	{
		private Guid uniqueId = Guid.NewGuid();
		private Type ancestorType;
		private IEnumerable<KeyValuePair<string, object>> metadata;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ancestorType">type to locate</param>
		/// <param name="metadata">metadata to locate</param>
		public SingletonPerAncestorLifestyle(Type ancestorType, object metadata = null)
		{
			this.ancestorType = ancestorType;

			if (metadata != null)
			{
				List<KeyValuePair<string, object>> newMetadata = new List<KeyValuePair<string, object>>();

				foreach (PropertyInfo runtimeProperty in metadata.GetType().GetRuntimeProperties())
				{
					if (!runtimeProperty.CanRead ||
					    !runtimeProperty.GetMethod.IsPublic ||
					    runtimeProperty.GetMethod.IsStatic)
					{
						continue;
					}

					newMetadata.Add(new KeyValuePair<string, object>(runtimeProperty.Name, runtimeProperty.GetValue(metadata)));
				}

				this.metadata = newMetadata;
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ancestorType">type to locate</param>
		/// <param name="metadata">metadata to locate</param>
		public SingletonPerAncestorLifestyle(Type ancestorType, IEnumerable<KeyValuePair<string, object>> metadata)
		{
			this.ancestorType = ancestorType;
			this.metadata = metadata;
		}

		/// <summary>
		/// Dispose of the lifestyle
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// Always transient
		/// </summary>
		public bool Transient
		{
			get { return true; }
		}

		/// <summary>
		/// Locate export
		/// </summary>
		/// <param name="creationDelegate"></param>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			CurrentInjectionInfo injectionInfo =
				injectionContext.GetInjectionStack().LastOrDefault(MatchInjectionInfo);

			if (injectionInfo == null)
			{
				throw new Exception("Could not locate matching ancestor");
			}

			string key = uniqueId + "|" + injectionInfo.UniqueId;

			object returnValue = injectionContext.GetExtraData(key);

			if (returnValue == null)
			{
				returnValue = creationDelegate(injectionScope, injectionContext);

				injectionContext.SetExtraData(key, returnValue);
			}

			return returnValue;
		}

		private bool MatchInjectionInfo(CurrentInjectionInfo arg)
		{
			bool found = false;

			if (ancestorType.GetTypeInfo().IsInterface)
			{
				if (ancestorType == arg.ActivationType)
				{
					found = true;
				}
				else if (arg.CurrentExportStrategy != null &&
							arg.CurrentExportStrategy.ExportTypes.Contains(ancestorType))
				{
					found = true;
				}
			}
			else
			{
				if (ancestorType == arg.ActivationType)
				{
					found = true;
				}
			}

			if (found && metadata != null)
			{
				if (arg.CurrentExportStrategy == null)
				{
					found = false;
				}
				else
				{
					foreach (KeyValuePair<string, object> keyValuePair in metadata)
					{
						object value;

						if (!arg.CurrentExportStrategy.Metadata.TryGetValue(keyValuePair.Key, out value) ||
							 !value.Equals(keyValuePair.Value))
						{
							found = false;
						}

						if (!found)
						{
							break;
						}
					}
				}
			}

			return found;
		}

		/// <summary>
		/// Clone lifestyle
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new SingletonPerAncestorLifestyle(ancestorType, metadata);
		}
	}
}
