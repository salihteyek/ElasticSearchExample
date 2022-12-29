using ElasticSearchExample.Application.Interfaces;
using ElasticSearchExample.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticSearchExample.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IIndexService, IndexService>();
			services.AddScoped(typeof(IDocumentService<>), typeof(DocumentService<>));
			services.AddSingleton<IElasticClient>(sp =>
			{
				string host = configuration.GetSection("ElasticSearchConfig:ConnectionString:Host").Value;
				string? userName = configuration.GetSection("ElasticSearchConfig:ConnectionString:UserName").Value;
				string? password = configuration.GetSection("ElasticSearchConfig:ConnectionString:Password").Value;

				var connectionSettings = new ConnectionSettings(new Uri(host));//.DefaultIndex("ProductIndex");

				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
					connectionSettings.BasicAuthentication(userName, password);

				return new ElasticClient(connectionSettings);
			});
		}
	}
}
