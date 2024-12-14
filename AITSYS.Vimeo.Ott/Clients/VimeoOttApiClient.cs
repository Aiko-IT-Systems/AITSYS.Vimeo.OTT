// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Logging;
using AITSYS.Vimeo.Ott.Rest;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Clients;

internal sealed class VimeoOttApiClient(VimeoOttClient client)
{
	internal VimeoOttClient Client { get; } = client;

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

	/*/// <summary>
	///     Executes a rest form data request.
	/// </summary>
	/// <param name="bucket">The bucket.</param>
	/// <param name="url">The url.</param>
	/// <param name="method">The method.</param>
	/// <param name="route">The route.</param>
	/// <param name="headers">The headers.</param>
	/// <param name="formData">The form data.</param>
	/// <param name="ratelimitWaitOverride">The ratelimit wait override.</param>
	private Task<RestResponse> DoFormRequestAsync(RateLimitBucket bucket, Uri url, RestRequestMethod method, string route, Dictionary<string, string> formData, Dictionary<string, string>? headers = null, double? ratelimitWaitOverride = null)
	{
		var req = new RestFormRequest(this.Client, bucket, url, method, route, formData, headers, ratelimitWaitOverride);

		this.RestClient.ExecuteFormRequestAsync(req).LogTaskFault(this.Client.Logger, LogLevel.Error, LoggerEvents.RestError, $"Error while executing request. Url: {url.AbsoluteUri}");

		return req.WaitForCompletionAsync();
	}*/

	internal void CanNotAccessEndpointWithCustomerAuthedClient()
	{
		if (!this.Client.CustomerBound)
			return;

		this.Client.Logger.LogCritical(LoggerEvents.RestError, "Tried to access a restricted endpoint with a customer bound client");
		throw new InvalidOperationException("A customer authenticated client can not access the current endpoint");
	}

	internal async Task<OttPagination<OttCustomersEmbeddedData>> GetCustomersAsync()
	{
		this.CanNotAccessEndpointWithCustomerAuthedClient();
		var route = $"{Endpoints.CUSTOMERS}";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
			{ }, out var path);

		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		return JsonConvert.DeserializeObject<OttPagination<OttCustomersEmbeddedData>>(res.Response)!;
	}

	internal async Task<OttCustomer<OttCustomerProductEmbeddedData>> GetCustomerAsync(int customerId)
	{
		var route = $"{Endpoints.CUSTOMERS}/:customer_id";
		var bucket = this.RestClient.GetBucket(RestRequestMethod.GET, route, new
		{
			customer_id = customerId
		}, out var path);

		var url = Utilities.GetApiUriFor(path);
		var res = await this.DoRequestAsync(bucket, url, RestRequestMethod.GET, route).ConfigureAwait(false);
		return JsonConvert.DeserializeObject<OttCustomer<OttCustomerProductEmbeddedData>>(res.Response)!;
	}
}
