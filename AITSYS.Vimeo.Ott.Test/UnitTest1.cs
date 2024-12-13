// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott;
using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.OTT.Test;

public sealed class Tests
{
	public VimeoOttClient ApiClient { get; set; }

	public VimeoOttClient CustomerApiClient { get; set; }

	public int UserId { get; set; }

	[SetUp]
	public void Setup()
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets<Tests>()
			.Build();
		this.ApiClient = new(new()
		{
			ApiKey = config["token"],
			MinimumLogLevel = LogLevel.Debug
		});

		this.UserId = Convert.ToInt32(config["user_id"]);

		this.CustomerApiClient = this.ApiClient.GetClientForCustomer($"https://api.vhx.tv/customers/{this.UserId}");
	}

	[Test]
	public void ThrowOnLockedEndpoint()
	{
		var exception = Assert.CatchAsync(this.CustomerApiClient.ApiClient.GetCustomersAsync);
		Console.WriteLine(exception.Message);
	}

	[Test]
	public async Task TestCustomerBoundApiClient()
	{
		var customer = await this.CustomerApiClient.ApiClient.GetCustomerAsync(this.UserId);
		var sku = customer.Embedded.Products.FirstOrDefault()?.Sku;
		Console.WriteLine(sku ?? "No sku");
	}

	[Test]
	public async Task TestPaginator()
	{
		var paginator = await this.ApiClient.ApiClient.GetCustomersAsync();
		foreach (var embeddedCustomer in paginator.Embedded.Customers)
		{
			Console.WriteLine(embeddedCustomer.Name);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Embedded.Product);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Topic);
		}
	}
}
