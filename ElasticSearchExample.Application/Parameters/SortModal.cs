using ElasticSearchExample.Application.Enums;

namespace ElasticSearchExample.Application.Parameters
{
	public class SortModal
	{
		public Dictionary<string, SortType> SortFields { get; set; }
	}
}
