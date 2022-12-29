using ElasticSearchExample.Application.Dtos;
using ElasticSearchExample.Application.Enums;
using ElasticSearchExample.Application.Interfaces;
using ElasticSearchExample.Application.Parameters;
using ElasticSearchExample.Domain.Base;
using Nest;

namespace ElasticSearchExample.Infrastructure.Services
{
	public class DocumentService<TEntitiy> : IDocumentService<TEntitiy> where TEntitiy : BaseEntitiy, new()
	{
		// not : eger connection string'de default client verilmisse indexName verilmesine gerek yok.
		private readonly IElasticClient _elasticClient;

		public DocumentService(IElasticClient elasticClient)
		{
			_elasticClient = elasticClient;
		}

		#region Command Operations
		public async Task<ResponseDto<TEntitiy>> InsertDocumentAsync(TEntitiy entitiy, string indexName)
		{
			// eger connection string'de default client verilmisse bu method kullanilabilir
			// await _elasticClient.IndexDocumentAsync(entitiy);
			// or
			var result = await _elasticClient.IndexAsync(entitiy, i => i.Index(indexName));
			return ResponseService.ReturnResult(entitiy, result);
		}

		public async Task<ResponseDto<List<TEntitiy>>> InsertManyDocumentAsync(List<TEntitiy> list, string indexName)
		{
			var result = await _elasticClient.IndexManyAsync(list, index: indexName);
			return ResponseService.ReturnResult(list, result);
		}

		public async Task<ResponseDto<TEntitiy>> UpdateDocumentAsync(TEntitiy entitiy, string indexName)
		{
			var result = await _elasticClient.UpdateAsync<TEntitiy>(entitiy.Id, u => u.Index(indexName).Doc(entitiy));
			// or
			// await _elasticClient.UpdateAsync<TEntitiy, dynamic>(entitiy.Id, u => u.Index(indexName).Doc(entitiy));
			return ResponseService.ReturnResult(entitiy, result);
		}

		public async Task<ResponseDto<TEntitiy>> UpsertDocumentAsync(TEntitiy entitiy, string indexName)
		{
			var result = await _elasticClient.UpdateAsync<TEntitiy>(entitiy.Id, u => u.Index(indexName).Doc(entitiy).Upsert(entitiy));
			return ResponseService.ReturnResult(entitiy, result);
		}

		public async Task<ResponseDto<int>> DeleteDocumentAsync(int id, string indexName)
		{
			var result = await _elasticClient.DeleteAsync<TEntitiy>(id, d => d.Index(indexName));
			// or
			// var result = await _elasticClient.DeleteAsync(new DeleteRequest(indexName, id));
			return ResponseService.ReturnResult(id, result);	
		}
		#endregion

		#region Query Operations
		public async Task<ResponseDto<TEntitiy>> GetDocumentByIdAsync(int id, string indexName)
		{
			var result = await _elasticClient.GetAsync<TEntitiy>(id, g => g.Index(indexName));
			return ResponseService.ReturnResult(result.Source, result);
		}

		public async Task<List<TEntitiy>> GetDocumentManyByIdAsync(List<long> list, string indexName)
		{
			var result = await _elasticClient.GetManyAsync<TEntitiy>(list, indexName);
			return result.Select(x => x.Source).ToList();
		}

		public async Task<List<TEntitiy>> SearchAsync(SearchModal searchModal, SortModal sortModal, string indexName)
		{
			var query = new List<QueryContainer>();
            if (searchModal?.Fields != null)
            {
				foreach (var item in searchModal.Fields)
				{
					
					query.Add(Query<TEntitiy>
						.Match(m => m
							.Field(new Field(item.Key.ToLower()))
							.Query(item.Value)
					));
					
				}
			}

			Func<SortDescriptor<dynamic>, IPromise<IList<ISort>>> sortList = st =>
			{
				if (sortModal?.SortFields != null)
				{
					foreach (var item in sortModal.SortFields)
					{
						if (item.Value == SortType.ASC)
							st.Ascending(item.Key);
						else
							st.Descending(item.Key);
					}
				}
				else
				{
					st.Ascending(SortSpecialField.Score);
				}
				return st;
			};

			var result = await _elasticClient.SearchAsync<TEntitiy>(s => s
				.Index(indexName)
				.From(searchModal.From)
				.Size(searchModal.Size)
				.Query(q => q
					.Bool(b => b.Must(query.ToArray()))
				)
				//.Sort(sortList)
			);

			return result.Documents.ToList();
		}

		public async Task<List<TEntitiy>> GetAllDocument(string indexName)
		{
			// size 
			var result = await _elasticClient.SearchAsync<TEntitiy>(s => s.Index(indexName).Scroll("2m").Size(1000).MatchAll());
			var documents = result.Documents.ToList();
			// or
			var result2 = await _elasticClient.SearchAsync<TEntitiy>(s => s.Index(indexName).MatchAll());
			var documents2 = result2.Documents.ToList();

			return documents;
		}

		#endregion
	}
}
