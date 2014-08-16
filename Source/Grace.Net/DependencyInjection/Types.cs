using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;
using Grace.Logging;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Static class that contains short cuts for finding classes to export
	/// </summary>
	public static class Types
	{
	    static Types()
	    {
	        DefaultTypeFilter = TypesThat.ArePublic();
	    } 

        /// <summary>
        /// Default type filter that will be used if consider is null
        /// TypesThat.ArePublic is the default behavior
        /// </summary>
        public static Func<Type, bool> DefaultTypeFilter { get; set; } 

		/// <summary>
		/// Returns the list of contained in the calling assembly
		/// </summary>
		/// <returns></returns>
        public static IEnumerable<Type> FromThisAssembly(Func<Type, bool> consider = null)
		{
		    if (consider == null)
		    {
		        consider = DefaultTypeFilter;
		    }

            return Assembly.GetCallingAssembly().DefinedTypes.Where(consider);
		}

		/// <summary>
		/// Returns the list of types from the Executing Assembly
		/// </summary>
		/// <returns></returns>
        public static IEnumerable<Type> FromExecutingAssembly(Func<Type, bool> consider = null)
		{
            if (consider == null)
            {
                consider = DefaultTypeFilter;
            }

            return Assembly.GetExecutingAssembly().DefinedTypes.Where(consider);
		}

		/// <summary>
		/// Returns the list of types from the Entry Assembly
		/// </summary>
		/// <returns></returns>
        public static IEnumerable<Type> FromEntryAssembly(Func<Type, bool> consider = null)
		{
            if (consider == null)
            {
                consider = DefaultTypeFilter;
            }

            return Assembly.GetEntryAssembly().DefinedTypes.Where(consider);
		}

        /// <summary>
        /// Returns a list of types from the currently loaded assemblies
        /// </summary>
        /// <param name="consider"></param>
        /// <returns></returns>
	    public static IEnumerable<Type> FromLoadedAssemblies(Func<Type, bool> consider = null)
	    {
	        if (consider == null)
	        {
	            consider = DefaultTypeFilter;
	        }

	        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.DefinedTypes).Where(consider);
	    }

	    /// <summary>
	    /// Gets a listed of exported types from an assembly
	    /// </summary>
	    /// <param name="assembly">assembly</param>
        /// <param name="consider">type filter</param>
	    /// <returns></returns>
        public static IEnumerable<Type> FromAssembly(Assembly assembly, Func<Type, bool> consider = null)
		{
            if (consider == null)
            {
                consider = DefaultTypeFilter;
            }

            return assembly.DefinedTypes.Where(consider);
		}

	    /// <summary>
	    /// Returns a list of types from a specified assembly
	    /// </summary>
	    /// <param name="assemblyPath">assembly path</param>
	    /// <param name="throwsException">throws exception if there is a load problem</param>
        /// <param name="consider">type filter</param>
	    /// <returns>list of types</returns>
        public static IEnumerable<Type> FromAssembly(string assemblyPath, bool throwsException = false, Func<Type, bool> consider = null)
		{
			Assembly loadedAssembly = LoadAssemblyFromPath(assemblyPath, throwsException);

	        if (loadedAssembly != null)
	        {
                if (consider == null)
                {
                    consider = DefaultTypeFilter;
                }

                return loadedAssembly.DefinedTypes.Where(consider);
	        }

			return ImmutableArray<Type>.Empty;
		}

		/// <summary>
		/// Allows the developer to pass a params list to the Register method
		/// </summary>
		/// <param name="types">list of types</param>
		/// <returns>list of types</returns>
		public static IEnumerable<Type> From(params Type[] types)
		{
			if (types.Length == 0)
			{
				throw new ArgumentNullException("types");
			}

			return types;
		}

	    /// <summary>
	    /// Returns a list of exported types from assemblies located in the directories provided
	    /// </summary>
	    /// <param name="directory">directory to scan</param>
	    /// <param name="throwsException">throws exception</param>
	    /// <param name="filter">dll filter</param>
	    /// <param name="consider">type filter</param>
	    /// <returns>list of types</returns>
	    public static IEnumerable<Type> FromDirectory(string directory,
			bool throwsException = false,
			Func<string, bool> filter = null,
            Func<Type, bool> consider = null)
		{
			List<Type> returnValue = new List<Type>();

			if (Directory.Exists(directory))
			{
				foreach (string file in Directory.GetFiles(directory, "*.dll"))
				{
					if (!file.EndsWith(".dll") || (filter != null && !filter(file)))
					{
						continue;
					}

					Assembly assembly = LoadAssemblyFromPath(file);

					if (assembly != null)
					{
					    if (consider == null)
					    {
					        consider = DefaultTypeFilter;
					    }
					   
                        returnValue.AddRange(assembly.ExportedTypes.Where(consider));
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Loads and assembly and returns null if there where exceptions
		/// </summary>
		/// <param name="path"></param>
		/// <param name="throwsException"></param>
		/// <returns></returns>
		public static Assembly LoadAssemblyFromPath(string path, bool throwsException = false)
		{
			Assembly returnValue = null;

			AssemblyName name = AssemblyName.GetAssemblyName(path);

			try
			{
				returnValue = Assembly.Load(name);
			}
			catch (Exception exp)
			{
				Logger.Error("Exception thrown while loading assembly: " + path, typeof(Types).FullName, exp);

				if (throwsException)
				{
					throw;
				}
			}

			return returnValue;
		}
	}
}