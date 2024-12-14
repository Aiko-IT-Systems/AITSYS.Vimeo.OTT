// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Net;

using AITSYS.Vimeo.Ott;
using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.OTT.Test;

public sealed class Tests
{
	public VimeoOttClient VimeoClient { get; set; }

	public VimeoOttClient CustomerVimeoClient { get; set; }

	public int UserId { get; set; }

	[SetUp]
	public void Setup()
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets<Tests>()
			.Build();
		this.VimeoClient = new(new()
		{
			ApiKey = config["token"],
			MinimumLogLevel = LogLevel.Debug,
			Proxy = new WebProxy("127.0.0.1:8000")
		});

		this.UserId = Convert.ToInt32(config["user_id"]);

		this.CustomerVimeoClient = this.VimeoClient.GetClientForCustomer($"https://api.vhx.tv/customers/{this.UserId}");
	}

	[Test]
	public void TestIntentionalExceptionThrow()
	{
		_ = Assert.CatchAsync(this.CustomerVimeoClient.ApiClient.GetCustomersAsync);
		_ = Assert.Catch(this.CustomerVimeoClient.ApiClient.CanNotAccessEndpointWithCustomerAuthedClient);
		Assert.DoesNotThrow(this.VimeoClient.ApiClient.CanNotAccessEndpointWithCustomerAuthedClient);
	}

	[Test]
	public async Task TestCustomerBoundApiClient()
	{
		var customer = await this.CustomerVimeoClient.ApiClient.GetCustomerAsync(this.UserId);
		var sku = customer.Embedded.Products.FirstOrDefault()?.Sku;
		Console.WriteLine(sku ?? "No sku");
		if (string.IsNullOrEmpty(sku))
			Assert.Fail();
	}

	[Test]
	public async Task TestPaginator()
	{
		var paginator = await this.VimeoClient.ApiClient.GetCustomersAsync();
		if (paginator.Count is 0)
			Assert.Fail();
		foreach (var embeddedCustomer in paginator.Embedded.Customers)
		{
			Console.WriteLine(embeddedCustomer.Name);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Embedded.Product);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Topic);
			Assert.Pass();
		}
	}
}