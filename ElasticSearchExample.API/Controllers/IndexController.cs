using ElasticSearchExample.API.Models.Index;
using ElasticSearchExample.Application.Interfaces;
using ElasticSearchExample.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchExample.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IndexController : ControllerBase
	{
		private readonly IIndexService _indexService;

		public IndexController(IIndexService indexService)
		{
			_indexService = indexService;
		}

		[HttpGet("any/{indexName}")]
		public async Task<IActionResult> Any(string indexName)
		{
			var response = await _indexService.AnyIndexAsync(indexName);
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateIndexModal modal)
		{
			var response = await _indexService.CreateIndexAsync<Product>(modal.IndexName);
			return Ok(response);
		}

		[HttpDelete("{indexName}")]
		public async Task<IActionResult> Delete(string indexName)
		{
			var response = await _indexService.DeleteIndexAsync(indexName);
			return Ok(response);
		}
	}
}
