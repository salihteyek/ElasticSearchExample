using ElasticSearchExample.API.Models.Document;
using ElasticSearchExample.Application.Constants;
using ElasticSearchExample.Application.Interfaces;
using ElasticSearchExample.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchExample.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		private readonly IDocumentService<Product> _documentService;

		public DocumentController(IDocumentService<Product> documentService)
		{
			_documentService = documentService;
		}

		[HttpPost("Insert")]
		public async Task<IActionResult> Insert(InsertDocumentModal modal)
		{
			var response = await _documentService.InsertDocumentAsync(modal.Product, modal.IndexName);
			return Ok(response);
		}

		[HttpPost("InsertMany")]
		public async Task<IActionResult> InsertMany(InsertManyDocumentModal modal)
		{
			var response = await _documentService.InsertManyDocumentAsync(modal.Products, modal.IndexName);
			return Ok(response);
		}

		[HttpPost("Update")]
		public async Task<IActionResult> Update(UpdateDocumentModal modal)
		{
			var response = await _documentService.UpdateDocumentAsync(new Product { Id = modal.Id, Name=modal.Name, Price=modal.Price}, modal.IndexName);
			return Ok(response);
		}

		[HttpPost("Upsert")]
		public async Task<IActionResult> Upsert(UpdateDocumentModal modal)
		{
			var response = await _documentService.UpsertDocumentAsync(new Product { Id = modal.Id, Name = modal.Name, Price = modal.Price }, modal.IndexName);
			return Ok(response);
		}

		[HttpPost("Delete")]
		public async Task<IActionResult> Delete(DeleteDocumentModal modal)
		{
			var response = await _documentService.DeleteDocumentAsync(modal.Id, modal.IndexName);
			return Ok(response);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id, string indexName)
		{
			var response = await _documentService.GetDocumentByIdAsync(id, indexName);
			return Ok(response);
		}

		[HttpPost("GetDocumentsByIds")]
		public async Task<IActionResult> GetDocumentsByIds(GetManyDocumentsModal modal)
		{
			var response = await _documentService.GetDocumentManyByIdAsync(modal.Ids, modal.IndexName);
			return Ok(response);
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(SearchDocumentModal modal)
		{
			var response = await _documentService.SearchAsync(modal.SearchModal, modal.SortModal, modal.IndexName);
			return Ok(response);
		}

		[HttpGet("GetAllDocuments/{indexName}")]
		public async Task<IActionResult> GetAllDocuments(string indexName)
		{
			var response = await _documentService.GetAllDocument(indexName);
			return Ok(response);
		}
	}
}
