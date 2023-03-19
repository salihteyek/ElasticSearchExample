using ElasticSearchExample.Application.Enums;

namespace ElasticSearchExample.Application.Parameters
{
	public class SearchModal
	{
		public int From { get; set; } = 0;
		public int Size { get; set; } = 50;
		
		public Dictionary<string, string>? EqualsFields { get; set; }

		public Dictionary<string, string>? ContainsFields { get; set; }

		public List<RangeFields>? RangeFields { get; set; }
	}

	public class RangeFields
	{
		public string ColumnName { get; set; }
        public ColumnType ColumnType { get; set; }
        public Dictionary<RangeCondition, object> RangeFilters { get; set; }
    }
}
