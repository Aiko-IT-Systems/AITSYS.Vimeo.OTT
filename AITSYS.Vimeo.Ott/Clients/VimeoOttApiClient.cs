// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Logging;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Clients;

internal sealed class VimeoOttApiClient(VimeoOttClient client)
{
	internal VimeoOttClient Client { get; } = client;

	internal HttpClient RestClient { get; } = client.RestClient;

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
		var result = await this.RestClient.GetStringAsync("/customers");
		return JsonConvert.DeserializeObject<OttPagination<OttCustomersEmbeddedData>>(result)!;
	}

	internal async Task<OttCustomer<OttCustomerProductEmbeddedData>> GetCustomerAsync(int customerId)
	{
		var result = await this.RestClient.GetStringAsync($"/customers/{customerId}");
		return JsonConvert.DeserializeObject<OttCustomer<OttCustomerProductEmbeddedData>>(result)!;
	}
}
