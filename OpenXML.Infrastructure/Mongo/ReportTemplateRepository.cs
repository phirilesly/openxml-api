using MongoDB.Driver;
using OpenXML.Abstractions.Enums;
using OpenXML.Abstractions.Models;
using OpenXML.Abstractions.Repository;


namespace OpenXML.Infrastructure.Mongo
{
     public class ReportTemplateRepository : IReportTemplateRepository
    {
        private MongoContext _context;
        public ReportTemplateRepository(MongoContext context)
        {
            _context = context;
        }

        public ReportTemplateRepository(string connectionString)
        {
            _context = new MongoContext(connectionString);
        }
        public async Task<IEnumerable<Template>> FindModelsAsync(List<SearchParameter> searchParameters)
        {
            FilterDefinition<Template> filter = Builders<Template>.Filter.Ne("isDeleted", true);
            foreach (var parameter in searchParameters.Where(
                    parameter => !string.IsNullOrEmpty(parameter.Name) && !string.IsNullOrEmpty(parameter.Value)))
            {
                var validParameter = Enum.TryParse(parameter.Name.ToUpper(), out SearchOptions option);
                if (!validParameter)
                {
                    continue;
                }
                switch (option)
                {
                    case SearchOptions.ID:
                        if (filter == null)
                        {
                            filter = Builders<Template>.Filter.Eq(x => x.Id, Guid.Parse(parameter.Value));
                        }
                        else
                        {
                            filter = Builders<Template>.Filter.Eq(x => x.Id, Guid.Parse(parameter.Value)) & filter;
                        }
                        break;
                }

            }
            if (filter == null) throw new ArgumentException("Invalid search parameters specified");
            List<Template> result = await _context.Templates.Find(filter).ToListAsync();
            return result;
        }

        public async Task<Template> LoadModelAsync(Guid modelId)
        {
            var filter = Builders<Template>.Filter.Eq("_id", modelId);
            filter = Builders<Template>.Filter.Ne("isDeleted", true) & filter;
            if (filter == null)
            {
                throw new ArgumentException("Invalid  search parameters specified");
            }
            var result = await _context.Templates.FindAsync(filter);
            return result.FirstOrDefault();
        }

        public async Task<Guid> SaveModelAsync(Template model)
        {
            FilterDefinition<Template> filter = Builders<Template>.Filter.Eq("_id", model.Id);

            var result = await _context.Templates.FindAsync(filter);

            if (result.Any())
            {
                await _context.Templates.ReplaceOneAsync(filter, model);
            }
            else
            {
                await _context.Templates.InsertOneAsync(model);
            }

            return model.Id;
        }
    }
}
