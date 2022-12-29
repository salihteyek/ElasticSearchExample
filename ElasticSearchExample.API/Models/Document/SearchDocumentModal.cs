using ElasticSearchExample.Application.Enums;
using ElasticSearchExample.Application.Parameters;

namespace ElasticSearchExample.API.Models.Document
{
	public class SearchDocumentModal : BaseDocumentModal
	{
        public SearchModal? SearchModal { get; set; }
        public SortModal? SortModal { get; set; }
    }
}
