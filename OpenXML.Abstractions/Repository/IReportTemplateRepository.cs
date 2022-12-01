using OpenXML.Abstractions.Models;

namespace OpenXML.Abstractions.Repository
{
    public interface IReportTemplateRepository : IQueryModelRepository<Template, Guid>
    {
    }
}
