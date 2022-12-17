using ElasticSearchExample.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchExample.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IIndexService _indexService;

		public ProductController(IIndexService indexService)
		{
			_indexService = indexService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok();
		}
	}
}
