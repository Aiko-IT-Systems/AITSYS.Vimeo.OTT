// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.OTT.Entities;
using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;

namespace AITSYS.Vimeo.Ott;

public sealed class VimeoOttClient
{
	public VimeoOttClient(VimeoOttConfiguration configuration)
	{
		this.Configuration = configuration;
		this.ClientHandler = new()
		{
			Proxy = this.Configuration.Proxy
		};
		this.RestClient = new(this.ClientHandler)
		{
			BaseAddress = new("https://api.vhx.tv")
		};

		if (this.Configuration.VhxCustomer is not null)
			this.RestClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Customer", this.Configuration.VhxCustomer);
		if (this.Configuration.VhxClientIp is not null)
			this.RestClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Client-IP", this.Configuration.VhxCustomer);

		this.RestClient.DefaultRequestHeaders.Authorization = new("Basic", $"{this.Configuration.ApiKey}:");
	}

	internal VimeoOttConfiguration Configuration { get; set; }

	internal HttpClientHandler ClientHandler { get; set; }
	internal HttpClient RestClient { get; set; }
}
