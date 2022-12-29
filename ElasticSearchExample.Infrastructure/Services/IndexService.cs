using ElasticSearchExample.Application.Dtos;
using ElasticSearchExample.Application.Interfaces;
using Nest;

namespace ElasticSearchExample.Infrastructure.Services
{
	public class IndexService : IIndexService
	{
		private readonly IElasticClient _elasticClient;
		public IndexService(IElasticClient elasticClient)
		{
			_elasticClient = elasticClient;
		}

		public async Task<ResponseDto<bool>> AnyIndexAsync(string indexName)
		{
			var anyIndex = await _elasticClient.Indices.ExistsAsync(indexName);
			if (anyIndex.Exists && anyIndex.IsValid)
				return ResponseService.ReturnResult(true, anyIndex);
			return ResponseService.ReturnResult(false, anyIndex);
		}

		public async Task<ResponseDto<bool>> CreateIndexAsync<Ttype>(string indexName) where Ttype : class
		{
			var anyIndex = await _elasticClient.Indices.ExistsAsync(indexName);
			if (anyIndex.Exists)
				return ResponseService.ReturnResult(true, anyIndex);

			// var response = await _elasticClient.Indices.CreateAsync(indexName); //or
			var result = await _elasticClient.Indices.CreateAsync(indexName,
				request => request
					.Index(indexName)
					.Map<Ttype>(m => m.AutoMap())
					//.Mappings(ms => ms.Map<Ttype>(m => m.AutoMap()))
					//.Map<Product>(m => m.AutoMap())
					.Settings(s => s.NumberOfShards(3).NumberOfReplicas(1))
			);
			if (result.ApiCall.Success != true)
				return ResponseService.ReturnResult(false, result);
			return ResponseService.ReturnResult(true, result);
		}

		public async Task<ResponseDto<bool>> DeleteIndexAsync(string indexName)
		{
			var result = await _elasticClient.Indices.DeleteAsync(indexName);
			if (result.ApiCall.Success != true)
				return ResponseService.ReturnResult(false, result);
			return ResponseService.ReturnResult(true, result);
		}
	}
}
