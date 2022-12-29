using ElasticSearchExample.Application.Dtos;

namespace ElasticSearchExample.Application.Interfaces
{
	public interface IIndexService
	{
		Task<ResponseDto<bool>> AnyIndexAsync(string indexName);
		Task<ResponseDto<bool>> CreateIndexAsync<Ttype>(string indexName) where Ttype : class;
		Task<ResponseDto<bool>> DeleteIndexAsync(string indexName);
	}
}
