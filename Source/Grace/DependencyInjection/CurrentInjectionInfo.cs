using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Information of the object being injected
	/// </summary>
	public class CurrentInjectionInfo
	{
		private Guid? uniqueId;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="activationType">type being exported</param>
		/// <param name="currentExportStrategy">strategy being injected</param>
		public CurrentInjectionInfo(Type activationType, IExportStrategy currentExportStrategy)
		{
			ActivationType = activationType;
			CurrentExportStrategy = currentExportStrategy;
		}

		/// <summary>
		/// Type being activated
		/// </summary>
		public Type ActivationType { get; private set; }

		/// <summary>
		/// Current injection strategy
		/// </summary>
		public IExportStrategy CurrentExportStrategy { get; private set; }

		/// <summary>
		/// Provides a unique id for this instance
		/// </summary>
		public Guid UniqueId
		{
			get
			{
				if (!uniqueId.HasValue)
				{
					uniqueId = Guid.NewGuid();
				}

				return uniqueId.Value;
			}
		}
	}
}
