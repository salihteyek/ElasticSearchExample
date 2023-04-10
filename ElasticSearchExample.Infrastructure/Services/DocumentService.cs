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


		/*
		 #region the way 1
			var keyword1 = "2";
			var result1 = await _elasticClient.SearchAsync<TEntitiy>(
				 s => s.Index(indexName)
				 .Query(
					 q => q.QueryString(
						 d => d.Query('*' + keyword1 + '*')
					 )).Size(5000));

			// return result.Documents.ToList();
			#endregion
		 */
		public async Task<List<TEntitiy>> SearchAsync(SearchModal searchModal, SortModal sortModal, string indexName)
		{
			// way 1 ustte

			/*
			 * match - Eşleşen veri döndürülür - equals gibi
			 * multi match - Çoklu alanlarda eşleşen verileri döner - çoklu equals
			 * match prefix'ler ise içeriyorsa döner - contains gibi
			 * term - eslesen kayıtları dondurur
			 */

			#region Query (Equals and Contains and Range)

			var query = new List<QueryContainer>();

			if (searchModal.EqualsFields != null)
			{
				foreach (var item in searchModal.EqualsFields)
				{
					query.Add(Query<TEntitiy>
						.Match(m => m
							.Field(new Field(item.Key.ToLower()))
							.Query(item.Value)
						)
					/*
					.Term(m => m

						.Field(new Field(item.Key.ToLower()))
						.Value(item.Value)

						/*
						.Field(new Field(item.Key.ToLower()))
						.Query(item.Value).MinimumShouldMatch(5)
						*/
					);

				}
			}

			if (searchModal.ContainsFields != null)
			{
				foreach (var item in searchModal.ContainsFields)
				{
					query.Add(Query<TEntitiy>
						.MatchBoolPrefix(m => m
							.Field(item.Key.ToLower())
							.Query(item.Value)
							//.Analyzer("standart")
						)
					);
				}
			}

			if (searchModal.RangeFields?.Count > 0)
			{
                foreach (var item in searchModal.RangeFields)
                {
					//query.Add(Query<TEntitiy>.Range(r => r.Field(item.ColumnName).GreaterThan((double)attr.Value)));

					if (item.ColumnType == ColumnType.Numeric)
					{
						NumericRangeQuery numericRangeQuery = new() { Field = item.ColumnName.ToLower(), Relation = RangeRelation.Within };
						foreach (var attr in item.RangeFilters)
                        {
							double.TryParse(attr.Value.ToString(), out double value);
							switch (attr.Key)
							{
								case RangeCondition.gt:
									
									numericRangeQuery.GreaterThan = value;
									break;
								case RangeCondition.gte:
									numericRangeQuery.GreaterThanOrEqualTo = value;
									break;
								case RangeCondition.lt:
									numericRangeQuery.LessThan = value;
									break;
								case RangeCondition.lte:
									numericRangeQuery.LessThanOrEqualTo = value;
									break;
								default:
									break;
							}
						}
						query.Add(numericRangeQuery);
                    }
					else
					{
						DateRangeQuery dateRangeQuery = new() { Field = item.ColumnName };
						foreach (var attr in item.RangeFilters)
						{
							DateTime.TryParse($"{DateTime.Parse(attr.Value.ToString()).ToString("s")}Z", out DateTime value);
							switch (attr.Key)
							{
								case RangeCondition.gt:
									dateRangeQuery.GreaterThan = value;
									break;
								case RangeCondition.gte:
									dateRangeQuery.GreaterThanOrEqualTo = value;
									break;
								case RangeCondition.lt:
									dateRangeQuery.LessThan = value;
									break;
								case RangeCondition.lte:
									dateRangeQuery.LessThanOrEqualTo = value;
									break;
								default:
									break;
							}
						}
						query.Add(dateRangeQuery);
                    }
                }
            }
			#endregion
			// Test
			#region Sorting
			Func<SortDescriptor<TEntitiy>, IPromise<IList<ISort>>> sort = st =>
			{
				if (sortModal?.SortFields?.Count > 0)
				{
					foreach (var item in sortModal.SortFields)
					{
						if (item.Value.Equals(SortType.ASC))
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
			#endregion

			var result = await _elasticClient.SearchAsync<TEntitiy>(s => s
				.Index(indexName)
				.From(searchModal.From)
				.Size(searchModal.Size)
				.Query(q => q
					.Bool(b => b
						.Should(query.ToArray())
					)
				)
				.Sort(sort)
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
