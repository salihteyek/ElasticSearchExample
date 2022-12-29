using ElasticSearchExample.Application.Dtos;
using ElasticSearchExample.Application.Parameters;
using ElasticSearchExample.Domain.Base;
using Nest;

namespace ElasticSearchExample.Application.Interfaces
{
	public interface IDocumentService<TEntitiy> where TEntitiy : BaseEntitiy, new()
	{
		Task<ResponseDto<TEntitiy>> InsertDocumentAsync(TEntitiy entitiy, string indexName);
		Task<ResponseDto<List<TEntitiy>>> InsertManyDocumentAsync(List<TEntitiy> list, string indexName);
		Task<ResponseDto<TEntitiy>> UpdateDocumentAsync(TEntitiy entitiy, string indexName);
		Task<ResponseDto<TEntitiy>> UpsertDocumentAsync(TEntitiy entitiy, string indexName);
		Task<ResponseDto<int>> DeleteDocumentAsync(int id, string indexName);

		Task<ResponseDto<TEntitiy>> GetDocumentByIdAsync(int id, string indexName);
		Task<List<TEntitiy>> GetDocumentManyByIdAsync(List<long> list, string indexName);
		Task<List<TEntitiy>> SearchAsync(SearchModal searchModal, SortModal sortModal, string indexName);

		Task<List<TEntitiy>> GetAllDocument(string indexName);
	}
}
