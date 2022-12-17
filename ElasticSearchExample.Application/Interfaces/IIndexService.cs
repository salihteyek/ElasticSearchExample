namespace ElasticSearchExample.Application.Interfaces
{
	public interface IIndexService
	{
		Task<bool> AnyIndexAsync(string indexName);
		Task CreateIndexAsync(string indexName);
		Task DeleteIndexAsync(string indexName);
	}
}
