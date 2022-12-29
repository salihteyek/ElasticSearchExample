using ElasticSearchExample.Domain.Entities;

namespace ElasticSearchExample.API.Models.Document
{
	public class InsertManyDocumentModal : BaseDocumentModal
	{
        public List<Product> Products { get; set; }
    }
}
