// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.OTT.Entities;

namespace AITSYS.Vimeo.Ott;

public sealed class VimeoOttClient(VimeoOttConfiguration configuration)
{
	internal VimeoOttConfiguration Configuration { get; set; } = configuration;

	public static void Test()
	{
		var customer = new OttCustomer<OttCustomerProductEmbeddedData>();
		var moviesUrl = customer.Embedded.Products.FirstOrDefault()?.Links.Movies.Href;
		Console.WriteLine(moviesUrl?.AbsoluteUri ?? "No movies url");

		var paginator = new OttPagination<OttCustomersEmbeddedData>();
		foreach (var embeddedCustomer in paginator.Embedded.Customers)
		{
			Console.WriteLine(embeddedCustomer.Name);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Topic);
		}

		var eventPaginator = new OttPagination<OttEventsEmbeddedData>();
		Console.WriteLine("Next page: {0}", eventPaginator.Links.Next?.Href?.AbsoluteUri ?? "none");
		foreach (var embeddedEvent in eventPaginator.Embedded.Events)
		{
			Console.WriteLine(embeddedEvent.Topic);
			Console.WriteLine(embeddedEvent.Data.Action);
		}
	}
}
