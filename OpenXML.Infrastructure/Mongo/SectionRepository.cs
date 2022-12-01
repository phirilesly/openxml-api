using MongoDB.Driver;
using OpenXML.Abstractions.Enums;
using OpenXML.Abstractions.Models;
using OpenXML.Abstractions.Repository;


namespace OpenXML.Infrastructure.Mongo
{
    public class SectionRepository : ISectionRepository
    {
        private MongoContext _context;
        public SectionRepository(MongoContext context)
        {
            _context = context;
        }

        public SectionRepository(string connectionString)
        {
            _context = new MongoContext(connectionString);
        }
        public async Task<IEnumerable<Section>> FindModelsAsync(List<SearchParameter> searchParameters)
        {
            FilterDefinition<Section> filter = Builders<Section>.Filter.Ne("isDeleted", true);
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
                            filter = Builders<Section>.Filter.Eq(x => x.Id, Guid.Parse(parameter.Value));
                        }
                        else
                        {
                            filter = Builders<Section>.Filter.Eq(x => x.Id, Guid.Parse(parameter.Value)) & filter;
                        }
                        break;
                }

            }
            if (filter == null) throw new ArgumentException("Invalid search parameters specified");
            List<Section> result = await _context.Sections.Find(filter).ToListAsync();
            return result;
        }

        public async Task<Section> LoadModelAsync(Guid modelId)
        {
            var filter = Builders<Section>.Filter.Eq("_id", modelId);
            filter = Builders<Section>.Filter.Ne("isDeleted", true) & filter;
            if (filter == null)
            {
                throw new ArgumentException("Invalid  search parameters specified");
            }
            var result = await _context.Sections.FindAsync(filter);
            return result.FirstOrDefault();
        }

        public async Task<Guid> SaveModelAsync(Section model)
        {
            FilterDefinition<Section> filter = Builders<Section>.Filter.Eq("_id", model.Id);

            var result = await _context.Sections.FindAsync(filter);

            if (result.Any())
            {
                await _context.Sections.ReplaceOneAsync(filter, model);
            }
            else
            {
                await _context.Sections.InsertOneAsync(model);
            }

            return model.Id;
        }
    }
}
