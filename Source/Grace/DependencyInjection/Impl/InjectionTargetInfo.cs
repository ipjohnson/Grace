using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents the information about injecting a particular object into another
	/// </summary>
	public class InjectionTargetInfo : IInjectionTargetInfo
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="injectionType">the type that is being injected into</param>
		/// <param name="injectionTypeAttributes">attributes on the type being injected</param>
		/// <param name="injectionTarget">the ParameterInfo or PropertyInfo being injected into</param>
        /// <param name="injectionDependencyType">injection dependency type</param>
		/// <param name="injectionTargetAttributes">attributes on the ParameterInfo or PropertyInfo</param>
		/// <param name="injectionMemberAttributes">attribute on the Method,Constructor, or Property</param>
		/// <param name="locateName">name used when locating</param>
		/// <param name="locateType">type used when locating</param>
        /// <param name="isRequired">is this injection required</param>
        /// <param name="defaultValue">default value for injection</param>
		public InjectionTargetInfo(Type injectionType,
			IEnumerable<Attribute> injectionTypeAttributes,
			object injectionTarget,
            ExportStrategyDependencyType injectionDependencyType,
            IEnumerable<Attribute> injectionTargetAttributes,
			IEnumerable<Attribute> injectionMemberAttributes,
			string locateName,
			Type locateType,
            bool isRequired,
            object defaultValue)
		{
			if (injectionType == null)
			{
				throw new ArgumentNullException("injectionType");
			}

			if (injectionTypeAttributes == null)
			{
				throw new ArgumentNullException("injectionTypeAttributes");
			}

			if (injectionTarget == null)
			{
				throw new ArgumentNullException("injectionTarget");
			}

			if (!(injectionTarget is ParameterInfo || injectionTarget is PropertyInfo))
			{
				throw new ArgumentException("injectionTarget must be PropertyInfo or ParameterInfo");
			}

			if (injectionTargetAttributes == null)
			{
				throw new ArgumentNullException("injectionTargetAttributes");
			}

			if (injectionMemberAttributes == null)
			{
				throw new ArgumentNullException("injectionMemberAttributes");
			}

			InjectionType = injectionType;
			InjectionTypeAttributes = injectionTypeAttributes;
			InjectionTarget = injectionTarget;
			InjectionTargetAttributes = injectionTargetAttributes;
			InjectionMemberAttributes = injectionMemberAttributes;
            InjectionDependencyType = injectionDependencyType;
			LocateName = locateName;
			LocateType = locateType;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
		}

		/// <summary>
		/// This is the type that is being injected into 
		/// </summary>
		public Type InjectionType { get; private set; }

		/// <summary>
		/// These are the attributes for the class that it's being injected into
		/// </summary>
		public IEnumerable<Attribute> InjectionTypeAttributes { get; private set; }

		/// <summary>
		/// The PropertyInfo or ParameterInfo that is being injected
		/// </summary>
		public object InjectionTarget { get; private set; }

        /// <summary>
        /// Type of injection that is being done
        /// </summary>
        public ExportStrategyDependencyType InjectionDependencyType { get; private set; }

        /// <summary>
        /// This is the property or parameter name being injected
        /// </summary>
        public string InjectionTargetName
		{
			get
			{
				ParameterInfo parameterInfo = InjectionTarget as ParameterInfo;

				return parameterInfo != null ? parameterInfo.Name : ((PropertyInfo)InjectionTarget).Name;
			}
		}

		/// <summary>
		/// This the type for the Property or Parameter being injected
		/// </summary>
		public Type InjectionTargetType
		{
			get
			{
				ParameterInfo parameterInfo = InjectionTarget as ParameterInfo;

				return parameterInfo != null ? parameterInfo.ParameterType : ((PropertyInfo)InjectionTarget).PropertyType;
			}
		}

		/// <summary>
		/// Attributes associated with the target PropertyInfo or ParameterInfo that is provided
		/// </summary>
		public IEnumerable<Attribute> InjectionTargetAttributes { get; private set; }

		/// <summary>
		/// Attributes associated with Constructor, Property or Method that is being injected
		/// </summary>
		public IEnumerable<Attribute> InjectionMemberAttributes { get; private set; }

		/// <summary>
		/// Locate name being used
		/// </summary>
		public string LocateName { get; private set; }

		/// <summary>
		/// Locate type being used
		/// </summary>
		public Type LocateType { get; private set; }

        /// <summary>
        /// Is this injection required
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Default value for injection
        /// </summary>
        public object DefaultValue { get; private set; }
	}
}