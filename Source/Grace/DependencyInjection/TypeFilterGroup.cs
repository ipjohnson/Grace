using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Groups together a set of type filters
	/// </summary>
	public class TypeFilterGroup
	{
		private readonly Func<Type, bool>[] typeFilters;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="typeFilters"></param>
		public TypeFilterGroup(params Func<Type, bool>[] typeFilters)
		{
			this.typeFilters = typeFilters;
		}

		/// <summary>
		/// Automatically convert from TypefilterGroup to Func(Type,bool)
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public static implicit operator Func<Type, bool>(TypeFilterGroup group)
		{
			return group.InternalFilter;
		}

		private bool InternalFilter(Type type)
		{
			foreach (Func<Type, bool> typeFilter in typeFilters)
			{
				if (!typeFilter(type))
				{
					return false;
				}
			}

			return true;
		}
	}
}