using ElasticSearchExample.Domain.Base;

namespace ElasticSearchExample.Domain.Entities
{
	public class Product : BaseEntitiy
	{
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
