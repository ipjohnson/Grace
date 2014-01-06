using System;
using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxInjectionTargetInfo : IInjectionTargetInfo
	{
		public Type InjectionType { get; set; }

		public IEnumerable<Attribute> InjectionTypeAttributes { get; set; }

		public object InjectionTarget { get; set; }

		public string InjectionTargetName { get; set; }

		public Type InjectionTargetType { get; set; }

		public IEnumerable<Attribute> InjectionTargetAttributes { get; set; }

		public IEnumerable<Attribute> InjectionMemberAttributes { get; set; }
	}
}