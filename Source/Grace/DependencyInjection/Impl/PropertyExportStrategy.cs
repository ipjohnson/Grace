using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Property Export exports a property from another IExportStrategy
	/// </summary>
	public class PropertyExportStrategy : ConfigurableExportStrategy
	{
		private readonly IExportCondition exportCondition;
		private readonly PropertyInfo propertyInfo;
		private readonly IExportStrategy strategy;
		private Func<object, object> propertyAccessor;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="property"></param>
		/// <param name="targetStrategy"></param>
		/// <param name="exportCondition"></param>
		public PropertyExportStrategy(PropertyInfo property,
			IExportStrategy targetStrategy,
			IExportCondition exportCondition) : base(property.PropertyType)
		{
			this.propertyInfo = property;
			this.strategy = targetStrategy;
			this.exportCondition = exportCondition;
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			if (propertyAccessor == null)
			{
				BuildAccessor();
			}
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			object targetOject = strategy.Activate(exportInjectionScope, context, consider, locateKey);

			if (targetOject != null)
			{
				return propertyAccessor(targetOject);
			}

			return null;
		}

		/// <summary>
		/// If this strategy meets condition and the target target strategy meets condition
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		public override bool MeetsCondition(IInjectionContext injectionContext)
		{
			if (exportCondition != null && !exportCondition.ConditionMeet(OwningScope, injectionContext, this))
			{
				return false;
			}

			return strategy.MeetsCondition(injectionContext);
		}

		/// <summary>
		/// Builds an accessor method 
		/// </summary>
		private void BuildAccessor()
		{
			ParameterExpression objectParam = Expression.Parameter(typeof(object));

			Expression converted = Expression.Convert(objectParam, propertyInfo.DeclaringType);

			Expression propertyAccess = Expression.Property(converted, propertyInfo);

			propertyAccessor = Expression.Lambda<Func<object, object>>(propertyAccess, objectParam).Compile();
		}
	}
}