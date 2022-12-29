namespace ElasticSearchExample.API.Models.Document
{
	public class GetManyDocumentsModal : BaseDocumentModal
	{
        public List<long> Ids { get; set; }
    }
}
