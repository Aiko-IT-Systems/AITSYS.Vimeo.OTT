using AITSYS.Vimeo.Ott;
using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Interfaces;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace TestNamespace;

public class TestClass
{
	public void TestFunction1()
	{
		var apiClient = new VimeoOttClient(new()
		{
			ApiKey = "test",
			MinimumLogLevel = LogLevel.Debug
		});

		var customerBoundApiClient = apiClient.GetClientForCustomer("https://api.vhx.tv/customers/1");
	}

	public void TestFunction2()
	{
		var customer = GetPaginator<OttCustomerProductEmbeddedData>("{}");
		var moviesUrl = customer.Embedded.Products.FirstOrDefault()?.Links.Movies.Href;
		Console.WriteLine(moviesUrl?.AbsoluteUri ?? "No movies url");

		var paginator = GetPaginator<OttCustomersEmbeddedData>("{}");
		foreach (var embeddedCustomer in paginator.Embedded.Customers)
		{
			Console.WriteLine(embeddedCustomer.Name);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Topic);
		}

		var eventPaginator = GetPaginator<OttEventsEmbeddedData>("{}");
		Console.WriteLine("Next page: {0}", eventPaginator.Links.Next?.Href?.AbsoluteUri ?? "none");
		foreach (var embeddedEvent in eventPaginator.Embedded.Events)
		{
			Console.WriteLine(embeddedEvent.Topic);
			Console.WriteLine(embeddedEvent.Data.Action);
		}
	}

	public static OttPagination<TOttEmbedded> GetPaginator<TOttEmbedded>(string data) where TOttEmbedded : IOttEmbedded
		=> JsonConvert.DeserializeObject<OttPagination<TOttEmbedded>>(data)!;
}
