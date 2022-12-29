using ElasticSearchExample.Application.Dtos;
using Nest;

namespace ElasticSearchExample.Infrastructure.Services
{
	internal class ResponseService
	{
		internal static ResponseDto<T> ReturnResult<T, ET>(T entity, ET elasticSearchResult) where ET : ResponseBase
		{
			return new ResponseDto<T>()
			{
				Data = entity,
				ErrorMessage = elasticSearchResult.ServerError?.Error?.Reason,
				IsValid = elasticSearchResult.IsValid,
				StatusCode = (int)elasticSearchResult.ApiCall.HttpStatusCode,
				Success = elasticSearchResult.ApiCall.Success
			};
		}
	}
}
