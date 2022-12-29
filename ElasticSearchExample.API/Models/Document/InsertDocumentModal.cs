using ElasticSearchExample.Domain.Entities;

namespace ElasticSearchExample.API.Models.Document
{
	public class InsertDocumentModal : BaseDocumentModal
	{
        public Product Product { get; set; }
    }
}
