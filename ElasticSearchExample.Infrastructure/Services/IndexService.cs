using ElasticSearchExample.Application.Interfaces;
using ElasticSearchExample.Domain.Entities;
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

		public async Task<bool> AnyIndexAsync(string indexName)
		{
			var anyIndex = await _elasticClient.Indices.ExistsAsync(indexName);
			if (anyIndex.Exists && anyIndex.IsValid)
				return true;
			return false;
		}

		public async Task CreateIndexAsync(string indexName)
		{
			var anyIndex = await _elasticClient.Indices.ExistsAsync(indexName);
			if (anyIndex.Exists)
				return;

			// var response = await _elasticClient.Indices.CreateAsync(indexName); //or
			await _elasticClient.Indices.CreateAsync(indexName,
				request => request
					.Index(indexName)
					.Map<Product>(m => m.AutoMap())
					/*
					.Mappings(ms => ms
						.Map<Product>(m => m.AutoMap()))
					*/
					.Settings(s => s.NumberOfShards(3).NumberOfReplicas(1))
			);
		}

		public async Task DeleteIndexAsync(string indexName)
		{
			await _elasticClient.Indices.DeleteAsync(indexName);
		}
	}
}
