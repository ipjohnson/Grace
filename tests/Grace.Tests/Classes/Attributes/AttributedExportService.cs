using Grace.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Tests.Classes.Attributes
{
    public interface IAttributedExportService
    {
        IAttributeBasicService BasicService { get; }
    }

    [Export(typeof(IAttributedExportService))]
    public class AttributedExportService : IAttributedExportService
    {
        [Export]
        public IAttributeBasicService BasicService
        {
            get
            {
                return new AttributeBasicService(); 
            }
        }
    }
}
