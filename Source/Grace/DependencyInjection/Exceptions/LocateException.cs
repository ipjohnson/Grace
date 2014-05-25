using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Utilities;
using JetBrains.Annotations;

namespace Grace.DependencyInjection.Exceptions
{
	public interface ILocationInformationEntry
	{
		void CreateMessage(StringBuilder stringBuilder);
	}

	/// <summary>
	/// Information about a strategy being activated
	/// </summary>
	public class StrategyBeingActivated : ILocationInformationEntry
	{
		private IExportStrategy exportStrategy;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportStrategy">strategy being activated</param>
		public StrategyBeingActivated(IExportStrategy exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Create message
		/// </summary>
		/// <param name="stringBuilder">string builder</param>
		public void CreateMessage(StringBuilder stringBuilder)
		{
			stringBuilder.AppendFormat("Activating type {0} ", exportStrategy.ActivationType.Name);

			if (exportStrategy.Lifestyle != null)
			{
				stringBuilder.AppendFormat("with lifestyle {0}",exportStrategy.Lifestyle.GetType().Name);
			}

			stringBuilder.AppendLine();
		}

		/// <summary>
		/// To String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			CreateMessage(builder);

			return builder.ToString();
		}
	}

	/// <summary>
	/// Information about a locate call
	/// </summary>
	public class InjectionScopeLocateEntry : ILocationInformationEntry
	{
		private string locateName;
		private Type locateType;
		private string scopeName;
		private bool hasFilter;
		private bool locateMultiple;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="locateName">name being located</param>
		/// <param name="locateType">type being located</param>
		/// <param name="scopeName">name of scope</param>
		/// <param name="hasFilter">has filter</param>
		/// <param name="locateMultiple">locate multiple</param>
		public InjectionScopeLocateEntry(string locateName, 
													Type locateType, 
													string scopeName,
													bool hasFilter, 
													bool locateMultiple)
		{
			this.locateName = locateName;
			this.locateType = locateType;
			this.scopeName = scopeName;
			this.hasFilter = hasFilter;
			this.locateMultiple = locateMultiple;
		}

		/// <summary>
		/// Create an error message
		/// </summary>
		/// <param name="stringBuilder">string builder for message</param>
		public void CreateMessage(StringBuilder stringBuilder)
		{
			if (locateMultiple)
			{
				stringBuilder.Append("LocateAll ");
			}
			else
			{
				stringBuilder.Append("Locate ");
			}

			if (locateName != null)
			{
				stringBuilder.AppendFormat("by name {0} ", locateName);
			}
			else
			{
				stringBuilder.AppendFormat("by type {0} ", locateType.Name);
			}

			stringBuilder.AppendFormat("in scope '{0}' ", scopeName);

			if (hasFilter)
			{
				stringBuilder.Append("with filter");
			}

			stringBuilder.AppendLine();
		}

		/// <summary>
		/// To String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			CreateMessage(builder);

			return builder.ToString();
		}
	}

	/// <summary>
	/// Location Information
	/// </summary>
	public class LocationInformationEntry : ILocationInformationEntry
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="locateName">name used to locate</param>
		/// <param name="locateType">type used to locate</param>
		/// <param name="targetInfo"></param>
		public LocationInformationEntry(string locateName, Type locateType, IInjectionTargetInfo targetInfo)
		{
			TargetInfo = targetInfo;
			LocateName = locateName;
			LocateType = locateType;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="locateName">name used to locate</param>
		/// <param name="locateType">type used to locate</param>
		/// <param name="targetInfo"></param>
		public LocationInformationEntry(string locateName, TypeWrapper locateType, IInjectionTargetInfo targetInfo)
		{
			TargetInfo = targetInfo;
			LocateName = locateName;
			LocateType = locateType;
		}

		/// <summary>
		/// Target Info
		/// </summary>
		public IInjectionTargetInfo TargetInfo { get; private set; }

		/// <summary>
		/// Locate Name used when locating
		/// </summary>
		public string LocateName { get; private set; }

		/// <summary>
		/// Type used when locating
		/// </summary>
		public Type LocateType { get; private set; }

		/// <summary>
		/// Create message
		/// </summary>
		/// <param name="stringBuilder"></param>
		public void CreateMessage(StringBuilder stringBuilder)
		{
			stringBuilder.Append("Importing  ");

			ParameterInfo parameterInfo = TargetInfo.InjectionTarget as ParameterInfo;
			PropertyInfo propertyInfo = TargetInfo.InjectionTarget as PropertyInfo;

			if (parameterInfo != null)
			{
				if (parameterInfo.Member is ConstructorInfo)
				{
					stringBuilder.AppendFormat("constructor parameter {0} ", parameterInfo.Name);
				}
				else
				{
					MethodInfo methodInfo = parameterInfo.Member as MethodInfo;

					if (methodInfo != null)
					{
						stringBuilder.AppendFormat("method parameter {0} on {1} ", parameterInfo.Name, methodInfo.Name);
					}
				}
			}
			else if (propertyInfo != null)
			{
				stringBuilder.AppendFormat("property {0} ", propertyInfo.Name);
			}

			if (LocateName != null)
			{
				stringBuilder.AppendFormat("using name {0}", LocateName);
			}
			else if (LocateType != null)
			{
				stringBuilder.AppendFormat("using type {0}", LocateType.Name);
			}

			stringBuilder.AppendLine();
		}

		/// <summary>
		/// To String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			CreateMessage(builder);

			return builder.ToString();
		}
	}

	/// <summary>
	/// Base exception for all exception that can be generated by Locate
	/// </summary>
	public class LocateException : Exception
	{
		private const int MaxLocationInfo = 30;
		private const int MaxExtraInfo = 30;
		private readonly List<ILocationInformationEntry> locationInformation = new List<ILocationInformationEntry>();
		private readonly Queue<ILocationInformationEntry> extraInfo = new Queue<ILocationInformationEntry>();
		private int dropped;
		private readonly string locateName;
		private readonly Type locatingType;
		private readonly IInjectionContext currentContext;

		/// <summary>
		/// Default Constructor takes the locating type and the current injection context
		/// </summary>
		/// <param name="locateName">the name used when locating</param>
		/// <param name="locatingType">the type used when locating</param>
		/// <param name="currentContext">the current context when the exception was generated</param>
		public LocateException(string locateName, Type locatingType, IInjectionContext currentContext)
		{
			this.locateName = locateName;
			this.locatingType = locatingType;
			this.currentContext = currentContext.Clone();
		}

		/// <summary>
		/// Constructor that takes inner exception
		/// </summary>
		/// <param name="locateName">locate name</param>
		/// <param name="locatingType">locate type</param>
		/// <param name="currentContext">injection context</param>
		/// <param name="innerException">inner exception</param>
		public LocateException(string locateName, Type locatingType, IInjectionContext currentContext, Exception innerException)
			: base("", innerException)
		{
			this.locateName = locateName;
			this.locatingType = locatingType;
			this.currentContext = currentContext.Clone();
		}

		/// <summary>
		/// Type that is being located, this can be null when locating something by name
		/// </summary>
		public Type LocatingType
		{
			get { return locatingType; }
		}

		/// <summary>
		/// This is the type that has the dependency on LocateType
		/// </summary>
		public Type OwningType
		{
			get
			{
				if (currentContext.TargetInfo != null)
				{
					return currentContext.TargetInfo.InjectionType;
				}

				return null;
			}
		}

		/// <summary>
		/// Injection context for exception
		/// </summary>
		protected IInjectionContext InjectionContext
		{
			get { return currentContext; }
		}

		/// <summary>
		/// The named that was used during location, this can be null when locating by type
		/// </summary>
		public string LocateName
		{
			get { return locateName; }
		}

		/// <summary>
		/// Adds a new location information entry
		/// </summary>
		/// <param name="entry">new entry</param>
		public void AddLocationInformationEntry(ILocationInformationEntry entry)
		{
			if (locationInformation.Count < MaxLocationInfo)
			{
				locationInformation.Add(entry);
			}
			else
			{
				extraInfo.Enqueue(entry);

				if (extraInfo.Count > MaxExtraInfo)
				{
					extraInfo.Dequeue();

					dropped++;
				}
			}
		}

		/// <summary>
		/// Location Information for this exception
		/// </summary>
		public IEnumerable<ILocationInformationEntry> LocationInformation
		{
			get { return locationInformation; }
		}

		/// <summary>
		/// Display string to be used for error messages
		/// </summary>
		protected string LocateDisplayString
		{
			get
			{
				if (LocatingType != null)
				{
					return locatingType.Name;
				}

				return LocateName;
			}
		}

		/// <summary>
		/// Creates a numbered exception message from the location information
		/// </summary>
		/// <param name="stringBuilder"></param>
		protected void CreateMessageFromLocationInformation(StringBuilder stringBuilder)
		{
			List<ILocationInformationEntry> extraEntries = new List<ILocationInformationEntry>(extraInfo);

			extraEntries.Reverse();

			BuildStringFromLocationInformationEntries(1, stringBuilder, extraEntries);

			if (dropped > 0)
			{
				stringBuilder.AppendLine();

				stringBuilder.AppendFormat("Dropped {0} entries {1}", dropped, Environment.NewLine);

				stringBuilder.AppendLine();
			}

			List<ILocationInformationEntry> entries = new List<ILocationInformationEntry>(locationInformation);

			entries.Reverse();

			BuildStringFromLocationInformationEntries(1 + extraInfo.Count + dropped, stringBuilder, entries);
		}

		private static void BuildStringFromLocationInformationEntries(int startingCount, StringBuilder stringBuilder, IEnumerable<ILocationInformationEntry> entries)
		{
			int i = startingCount;

			foreach (ILocationInformationEntry locationInformationEntry in entries)
			{
				stringBuilder.AppendFormat("{0} ", i);

				locationInformationEntry.CreateMessage(stringBuilder);

				i++;
			}
		}
	}
}
