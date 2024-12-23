// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Test;

public sealed class Tests
{
	public VimeoOttClient VimeoClient { get; set; }

	public VimeoOttClient CustomerVimeoClient { get; set; }

	public int UserId { get; set; }

	[OneTimeSetUp]
	public void Setup()
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets<Tests>()
			.Build();
		this.VimeoClient = new(new()
		{
			ApiKey = config["token"] ?? Environment.GetEnvironmentVariable("VIMEO_TEST_TOKEN"),
			MinimumLogLevel = LogLevel.Debug /*,
			Proxy = new WebProxy("127.0.0.1:8000")*/
		});

		this.UserId = Convert.ToInt32(config["user_id"] ?? Environment.GetEnvironmentVariable("VIMEO_TEST_USER"));

		this.CustomerVimeoClient = this.VimeoClient.GetClientForCustomer($"https://api.vhx.tv/customers/{this.UserId}");
	}

	[OneTimeTearDown]
	public void Dispose()
	{
		this.CustomerVimeoClient.Dispose();
		this.VimeoClient.Dispose();
	}

	[Test]
	public void TestIntentionalExceptionThrow()
	{
		_ = Assert.CatchAsync(async () => await this.CustomerVimeoClient.ApiClient.ListCustomersAsync());
		_ = Assert.Catch(this.CustomerVimeoClient.ApiClient.CanNotAccessEndpointWithCustomerAuthedClient);
		Assert.DoesNotThrow(this.VimeoClient.ApiClient.CanNotAccessEndpointWithCustomerAuthedClient);
	}

	[Test]
	public async Task TestCustomerBoundApiClient()
	{
		var customer = await this.CustomerVimeoClient.ApiClient.RetrieveCustomerAsync(this.UserId);
		Assert.Multiple(() =>
		{
			Assert.That(customer.Client, Is.Not.EqualTo(null));
			Assert.That(customer.Embedded.Products.All(p => p.Client is not null));
		});
		var sku = customer.Embedded.Products.FirstOrDefault()?.Sku;
		Console.WriteLine("----------------------------------");
		Console.WriteLine(sku ?? "No sku");
		if (string.IsNullOrEmpty(sku))
			Assert.Fail();
	}

	[Test]
	public Task TestVariousMethods()
	{
		Assert.DoesNotThrowAsync(async () =>
		{
			var customer = await this.CustomerVimeoClient.ApiClient.RetrieveCustomerAsync(this.UserId);
			var products = await customer.RetrieveProductsAsync();
			Assert.That(products, Is.Not.Empty);
			var events = await customer.RetrieveEventsAsync();
			Assert.That(events, Is.Not.Empty);
		});
		return Task.CompletedTask;
	}

	[Test]
	public async Task TestPaginator()
	{
		var paginator = await this.VimeoClient.ApiClient.ListCustomersAsync(status: "all");
		if (paginator.Count is 0)
			Assert.Fail();
		foreach (var embeddedCustomer in paginator.Embedded.Customers)
		{
			Assert.Multiple(() =>
			{
				Assert.That(embeddedCustomer.Client, Is.Not.EqualTo(null));
				Assert.That(embeddedCustomer.Embedded.LatestEvent.Client, Is.Not.EqualTo(null));
			});
			Console.WriteLine("----------------------------------");
			Console.WriteLine(embeddedCustomer.Name);
			Console.WriteLine(embeddedCustomer.Plan);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Embedded.Product);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Topic);
			Console.WriteLine(embeddedCustomer.Embedded.LatestEvent.Data.Status);
			var eventPaginator = await this.VimeoClient.ApiClient.RetrieveCustomerEventsAsync(embeddedCustomer.Id);
			if (eventPaginator.Count is 0)
				Assert.Fail();
			Assert.That(eventPaginator.Embedded.Events.All(e => e.Client is not null));
			var firstEvent = eventPaginator.Embedded.Events.First();
			Console.WriteLine(firstEvent.Embedded.Product?.Name);
			Console.WriteLine(firstEvent.Topic);
			Console.WriteLine(firstEvent.Data.Status);
		}
	}
}
