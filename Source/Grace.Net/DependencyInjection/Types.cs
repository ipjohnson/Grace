using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Grace.Logging;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Static class that contains short cuts for finding classes to export
	/// </summary>
	public static class Types
	{
		/// <summary>
		/// Returns the list of types contained in the calling assembly
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> FromThisAssembly()
		{
			return Assembly.GetCallingAssembly().ExportedTypes;
		}

		/// <summary>
		/// Returns the list of exported types from the Executing Assembly
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> FromExecutingAssembly()
		{
			return Assembly.GetExecutingAssembly().ExportedTypes;
		}

		/// <summary>
		/// Returns the list of exported types from the Entry Assembly
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> FromEntryAssembly()
		{
			return Assembly.GetEntryAssembly().ExportedTypes;
		}

		/// <summary>
		/// Gets a listed of exported types from an assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Type> FromAssembly(Assembly assembly)
		{
			return assembly.ExportedTypes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyPath"></param>
		/// <param name="throwsException"></param>
		/// <returns></returns>
		public static IEnumerable<Type> FromAssembly(string assemblyPath, bool throwsException = false)
		{
			Assembly loadedAssembly = LoadAssemblyFromPath(assemblyPath, throwsException);

			return loadedAssembly != null ? loadedAssembly.ExportedTypes : new Type[0];
		}

		/// <summary>
		/// Allows the developer to pass a params list to the Register method
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
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
		/// <param name="directory"></param>
		/// <param name="throwsException"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<Type> FromDirectory(string directory,
			bool throwsException = false,
			Func<string, bool> filter = null)
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
						returnValue.AddRange(assembly.ExportedTypes);
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