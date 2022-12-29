namespace ElasticSearchExample.Application.Parameters
{
	public class SearchModal
	{
		public int From { get; set; } = 0;
		public int Size { get; set; } = 50;
		public Dictionary<string, string> Fields { get; set; }
	}
}
