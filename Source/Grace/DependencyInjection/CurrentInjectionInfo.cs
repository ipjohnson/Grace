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
        private Type _activationType;
        private IExportStrategy _currentExportStrategy;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="activationType">type being exported</param>
		/// <param name="currentExportStrategy">strategy being injected</param>
		public CurrentInjectionInfo(Type activationType, IExportStrategy currentExportStrategy)
		{
			_activationType = activationType;
			_currentExportStrategy = currentExportStrategy;
		}

		/// <summary>
		/// Type being activated
		/// </summary>
		public Type ActivationType
        {
            get { return _activationType; }
        }

		/// <summary>
		/// Current injection strategy
		/// </summary>
		public IExportStrategy CurrentExportStrategy
        {
            get { return _currentExportStrategy; }
        }

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
