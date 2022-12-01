using OpenXML.Abstractions.Models;


namespace OpenXML.Abstractions.Repository
{
    public interface IQueryModelRepository<T, TId> where T : IQueryModel
    {
        Task<T> LoadModelAsync(TId modelId);
        Task<IEnumerable<T>> FindModelsAsync(List<SearchParameter> searchParameters);
        Task<TId> SaveModelAsync(T model);
    }
}
