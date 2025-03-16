// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Net;

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Entities.Products;
using AITSYS.Vimeo.Ott.Logging;
using AITSYS.Vimeo.Ott.Rest;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Clients;

/// <summary>
///     Represents a client for the Vimeo OTT API.
/// </summary>
/// <param name="client">The client.</param>
internal sealed class VimeoOttApiClient(VimeoOttClient client)
{
	/// <summary>
	///     Gets the client.
	/// </summary>
	internal VimeoOttClient Client { get; } = client;

	/// <summary>
	///     Gets the rest client.
	/// </summary>
	internal RestClient RestClient { get; } = new(client);

	/// <summary>
	///     Executes a rest request.
	/// </summary>
	/// <param name="bucket">The bucket.</param>
	/// <param name="url">The url.</param>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <param name="headers">The headers.</param>
	/// <param name="payload">The payload.</param>
	/// <param name="ratelimitWaitOverride">The ratelimit wait override.</param>
	private Task<RestResponse> DoRequestAsync(RateLimitBucket bucket, Uri url, RestRequestMethod method, string route, IReadOnlyDictionary<string, string>? headers = null, string? payload = null, double? ratelimitWaitOverride = null)
	{
		var req = new RestRequest(this.Client, bucket, url, method, route, headers, payload, ratelimitWaitOverride);

		this.RestClient.ExecuteRequestAsync(req).LogTaskFault(this.Client.Logger, LogLevel.Error, LoggerEvents.RestError, $"Error while executing request. Url: {url.AbsoluteUri}");

		return req.WaitForCompletionAsync();
	}

	/// <summary>
	///     Returns an execption, if a customer authenticated client tries to access a restricted endpoint.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     Thrown when a customer authenticated client tries to access a restricted
	///     endpoint.
	/// </exception>
	internal void CanNotAccessEndpointWithCustomerAuthedClient()
	{
		if (!this.Client.CustomerBound)
			return;

		this.Client.Logger.LogCritical(LoggerEvents.RestError, "Tried to access a restricted endpoint with a customer bound client");
		throw new InvalidOperationException("A customer authenticated client can not access the current endpoint");
	}

	/// <summary>
	///     Executes a raw request.
	/// </summary>
	/// <param name="url">The url to call.</param>
	/// <param name="method">The method to use.</param>
	/// <param name="payload">The payload to send.</param>
	/// <returns>The response body.</returns>
	internal async Task<string> ExecuteRawRequestAsync(string url, RestRequestMethod method, string? payload = null)
	{
		var res = await this.DoRequestAsync(new("unlimited", null!), new(url), method, "unlimited", payload is not null
			? new Dictionary<string, string>
			{
				["Content-Type"] = "application/json"
			}
			: null, payload).ConfigureAwait(false);
		return res.Response;
	}

	/// <summary>
	///     Customers can be listed for your account or for a given product.
	/// </summary>
	/// <param name="productId">The <c>id</c> of a product.</param>
	/// <param name="email">The email address to find in the paginated results.</param>
	/// <param name="query">
	///     The query to search and filter the paginated results. By default filters for customers with status
	///     of <c>enabled</c> (subscribed). The status query param should be explicitly set in order to include customers that
	///     are not <c>enabled</c>. See status below.
	/// </param>
	/// <param name="sort">The sort to order the results. Options are <c>newest</c>, <c>oldest</c>, or <c>latest</c>.</param>
	/// <param name="status">
	///     The status indicates the subscription status of the customer. Options are <c>enabled</c>,
	///     <c>disabled</c>, <c>cancelled</c>, <c>refunded</c>, <c>expired</c>, <c>paused</c>, or <c>all</c> (includes all
	///     statuses).
	/// </param>
	/// <param name="page">The page number of the paginated result.</param>
	/// <param name="perPage">The page size of the paginated result.</param>
	/// <returns>A paginated result.</returns>
	internal async Task<OttPagination<OttCustomersEmbeddedData>> ListCustomersAsync(int? productId = null, string? email = null, string? query = null, string? sort = null, string? status = null, int page = 1, int perPage = 50)
	{
		this.CanNotAccessEndpointWithCustomerAuthedClient();
		var route = $"{Endpoints.CUSTOMERS}";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
			{ }, out var path);
		var url = Utilities.GetApiUriFor(path).AddParameter(nameof(page), page.ToString()).AddParameter("per_page", perPage.ToString());
		if (productId is not null)
			url = url.AddParameter("product", $"{Utilities.GetApiBaseUri()}{Endpoints.PRODUCTS}/{productId}");
		if (email is not null)
			url = url.AddParameter(nameof(email), email);
		if (query is not null)
			url = url.AddParameter(nameof(query), query);
		if (status is not null)
			url = url.AddParameter(nameof(status), status);
		if (sort is not null)
			url = url.AddParameter(nameof(sort), sort);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		var paginator = JsonConvert.DeserializeObject<OttPagination<OttCustomersEmbeddedData>>(res.Response)!;
		paginator.Client = this.Client;
		if (paginator.Embedded is not null)
			foreach (var customer in paginator.Embedded.Customers)
			{
				customer.Client = this.Client;
				if (customer.Embedded?.LatestEvent is not null)
					customer.Embedded.LatestEvent.Client = this.Client;
			}

		return paginator;
	}

	/// <summary>
	///     Retrieves an existing customer. You can optionally specify a product parameter to scope the customer retrieval to
	///     it (ie. “Is this customer subscribed to this product?”).
	/// </summary>
	/// <param name="customerId">The <c>id</c> of the customer being retrieved.</param>
	/// <param name="productId">The <c>id</c> of a product.</param>
	/// <returns>The requested customer.</returns>
	internal async Task<OttCustomer<OttCustomerProductEmbeddedData>?> RetrieveCustomerAsync(int customerId, int? productId = null)
	{
		var route = $"{Endpoints.CUSTOMERS}/:customer_id";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
		{
			customer_id = customerId
		}, out var path);
		var url = Utilities.GetApiUriFor(path);
		if (productId is not null)
			url = url.AddParameter("product", $"{Utilities.GetApiBaseUri()}{Endpoints.PRODUCTS}/{productId}");
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		if (res.ResponseCode == HttpStatusCode.NotFound)
			return null;

		var obj = JsonConvert.DeserializeObject<OttCustomer<OttCustomerProductEmbeddedData>>(res.Response)!;
		obj.Client = this.Client;
		foreach (var product in obj.Embedded.Products)
			product.Client = this.Client;
		return obj;
	}

	/// <summary>
	///     Updates an existing customer.
	/// </summary>
	/// <param name="customerId">The <c>id</c> of the customer being updated.</param>
	/// <param name="name">The new name of the customer.</param>
	/// <param name="password">The new password of the customer.</param>
	/// <returns>The updated customer.</returns>
	internal async Task<OttCustomer<OttCustomerProductEmbeddedData>?> UpdateCustomerAsync(int customerId, string? name = null, string? password = null)
	{
		if (name is null && password is null)
			throw new ArgumentException("At least one parameter must be set", $"{nameof(name)}, {nameof(password)}");

		var route = $"{Endpoints.CUSTOMERS}/:customer_id";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.PATCH, route, new
		{
			customer_id = customerId
		}, out var path);

		RestCustomerUpdatePayload payload = new()
		{
			Name = name,
			Password = password
		};

		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.PATCH, route, payload: JsonConvert.SerializeObject(payload, Formatting.Indented)).ConfigureAwait(false);
		if (res.ResponseCode == HttpStatusCode.NotFound)
			return null;

		var obj = JsonConvert.DeserializeObject<OttCustomer<OttCustomerProductEmbeddedData>>(res.Response)!;
		obj.Client = this.Client;
		foreach (var product in obj.Embedded.Products)
			product.Client = this.Client;
		return obj;
	}

	/// <summary>
	///     Retrieves events of an existing customers.
	/// </summary>
	/// <param name="customerId">The <c>id</c> of the customer events are retrieved for.</param>
	/// <returns>The requested events.</returns>
	internal async Task<OttPagination<OttEventsEmbeddedData<OttEventProductObjectEmbeddedData>>> RetrieveCustomerEventsAsync(int customerId)
	{
		var route = $"{Endpoints.CUSTOMERS}/:customer_id{Endpoints.EVENTS}";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
		{
			customer_id = customerId
		}, out var path);
		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		var paginator = JsonConvert.DeserializeObject<OttPagination<OttEventsEmbeddedData<OttEventProductObjectEmbeddedData>>>(res.Response)!;
		paginator.Client = this.Client;
		foreach (var @event in paginator.Embedded.Events)
			@event.Client = this.Client;
		return paginator;
	}

	/// <summary>
	///     Retrieves products of an existing customers.
	/// </summary>
	/// <param name="customerId">The <c>id</c> of the customer are being retrieved for.</param>
	/// <returns>The requested products.</returns>
	internal async Task<OttPagination<OttCustomerProductEmbeddedData>> RetrieveCustomerProductsAsync(int customerId)
	{
		var route = $"{Endpoints.CUSTOMERS}/:customer_id{Endpoints.PRODUCTS}";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
		{
			customer_id = customerId
		}, out var path);
		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		var paginator = JsonConvert.DeserializeObject<OttPagination<OttCustomerProductEmbeddedData>>(res.Response)!;
		paginator.Client = this.Client;
		foreach (var product in paginator.Embedded.Products)
			product.Client = this.Client;
		return paginator;
	}

	// https://api.vhx.tv/customers/40035072/watching
	// https://api.vhx.tv/customers/40035072/watchlist

	/// <summary>
	///     Products can be listed for your account.
	/// </summary>
	/// <param name="query">The query to search and filter the results.</param>
	/// <param name="active"></param>
	/// <param name="sort">
	///     The sort to order the results. Options are <c>alphabetical</c>, <c>newest</c>, <c>oldest</c>, or
	///     <c>position</c>.
	/// </param>
	/// <param name="page">The page number of the paginated result.</param>
	/// <param name="perPage">The page size of the paginated result.</param>
	/// <returns>A paginated result.</returns>
	internal async Task<OttPagination<OttProductsEmbeddedData>> ListProductsAsync(string? query = null, bool? active = null, string? sort = null, int page = 1, int perPage = 50)
	{
		this.CanNotAccessEndpointWithCustomerAuthedClient();
		var route = $"{Endpoints.PRODUCTS}";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
			{ }, out var path);
		var url = Utilities.GetApiUriFor(path).AddParameter(nameof(page), page.ToString()).AddParameter("per_page", perPage.ToString());
		if (query is not null)
			url = url.AddParameter(nameof(query), query);
		if (active is not null)
			url = url.AddParameter(nameof(active), active.Value.ToString());
		if (sort is not null)
			url = url.AddParameter(nameof(sort), sort);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		var paginator = JsonConvert.DeserializeObject<OttPagination<OttProductsEmbeddedData>>(res.Response)!;
		paginator.Client = this.Client;
		foreach (var product in paginator.Embedded.Products)
			product.Client = this.Client;
		return paginator;
	}

	/// <summary>
	///     Retrieves an existing product.
	/// </summary>
	/// <param name="productId">The <c>id</c> of the product being retrieved.</param>
	/// <returns>The requested product.</returns>
	internal async Task<OttProduct<OttProductEmbeddedData>?> RetrieveProductAsync(int productId)
	{
		var route = $"{Endpoints.PRODUCTS}/:product_id";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
		{
			product_id = productId
		}, out var path);
		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		if (res.ResponseCode == HttpStatusCode.NotFound)
			return null;

		var obj = JsonConvert.DeserializeObject<OttProduct<OttProductEmbeddedData>>(res.Response)!;
		obj.Client = this.Client;
		return obj;
	}
}
