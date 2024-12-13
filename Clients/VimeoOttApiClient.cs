// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Clients;

internal sealed class VimeoOttApiClient(VimeoOttClient client)
{
	internal VimeoOttClient Client { get; } = client;

	internal HttpClient RestClient { get; } = client.RestClient;

	internal void CanNotAccessEndpointWithCustomerAuthedClient()
	{
		if (this.Client.CustomerBound)
			throw new InvalidOperationException("A customer authenticated client can not access the current endpoint");
	}

	internal async Task<OttPagination<OttCustomersEmbeddedData>> GetCustomersAsync()
	{
		this.CanNotAccessEndpointWithCustomerAuthedClient();
		var result = await this.RestClient.GetStringAsync("/customers");
		return JsonConvert.DeserializeObject<OttPagination<OttCustomersEmbeddedData>>(result)!;
	}
}
