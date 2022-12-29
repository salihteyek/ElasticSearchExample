using ElasticSearchExample.Domain.Entities;

namespace ElasticSearchExample.API.Models.Document
{
	public class UpdateDocumentModal : BaseDocumentModal
	{
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
    }
}
