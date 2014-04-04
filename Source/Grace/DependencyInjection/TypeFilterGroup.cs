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
			if (typeFilters == null)
			{
				throw new ArgumentNullException("typeFilters");
			}

			this.typeFilters = typeFilters;
		}

		/// <summary>
		/// Or together the filters rather than And them
		/// </summary>
		public bool UseOr { get; set; }

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
			if (UseOr)
			{
				foreach (Func<Type, bool> typeFilter in typeFilters)
				{
					if (typeFilter(type))
					{
						return true;
					}
				}

				return false;
			}

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