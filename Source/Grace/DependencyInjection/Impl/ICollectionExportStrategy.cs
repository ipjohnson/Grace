using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Creates a new instance of any class that implements ICollection(TItem)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TItem"></typeparam>
// ReSharper disable once InconsistentNaming
	public class ICollectionExportStrategy<T, TItem> : ConfigurableExportStrategy
	{
		private Func<ICollection<TItem>> activateT;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ICollectionExportStrategy()
			: base(typeof(T))
		{
		}

		/// <summary>
		/// Export names for this collection
		/// </summary>
		public override IEnumerable<string> ExportNames
		{
			get { yield return typeof(T).FullName; }
		}

		/// <summary>
		/// Activate the strategy
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			ICollection<TItem> collection = activateT();

			foreach (TItem item in exportInjectionScope.Locate<IEnumerable<TItem>>(context, consider, locateKey))
			{
				collection.Add(item);
			}

			return collection;
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public override void Initialize()
		{
			ConstructorInfo info =
				_exportType.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => x.GetParameters().Length == 0);

			if (info == null)
			{
				throw new Exception("Type does not have a default constructor: " + _exportType.FullName);
			}

			Expression newExpression = Expression.New(info);

			activateT = Expression.Lambda<Func<ICollection<TItem>>>(newExpression).Compile();

			base.Initialize();
		}

		/// <summary>
		/// Override to compare 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ICollectionExportStrategy<T, TItem> strategy = obj as ICollectionExportStrategy<T, TItem>;

			if (strategy != null && strategy.OwningScope == OwningScope)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Overriding because I'm overriding equals
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return ActivationName.GetHashCode();
		}
	}
}