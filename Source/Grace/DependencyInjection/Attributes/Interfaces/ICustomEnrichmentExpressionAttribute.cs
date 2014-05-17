using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributed that implement this interface will be used to enrich the delegate
	/// </summary>
	public interface ICustomEnrichmentExpressionAttribute
	{
		/// <summary>
		/// Get a custom enrichment provider, you are given the attributed type and possible attributed member
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <param name="attributedMember">PropertyInfo or MethodInfo that was attributed, can be null if the class was attributed</param>
		/// <returns>custom linq expression provider</returns>
		ICustomEnrichmentLinqExpressionProvider GetProvider(Type attributedType, object attributedMember);
	}
}
