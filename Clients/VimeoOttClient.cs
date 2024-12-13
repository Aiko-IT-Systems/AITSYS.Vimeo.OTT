// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.OTT.Entities;

namespace AITSYS.Vimeo.Ott.Clients;

public sealed class VimeoOttClient
{
	public VimeoOttClient(VimeoOttConfiguration configuration)
	{
		this.Configuration = configuration;
		this.RestClient = new(new HttpClientHandler
		{
			Proxy = this.Configuration.Proxy
		})
		{
			BaseAddress = new("https://api.vhx.tv")
		};

		if (this.Configuration.VhxCustomer is not null)
			this.RestClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Customer", this.Configuration.VhxCustomer);
		if (this.Configuration.VhxClientIp is not null)
			this.RestClient.DefaultRequestHeaders.TryAddWithoutValidation("VHX-Client-IP", this.Configuration.VhxCustomer);

		this.RestClient.DefaultRequestHeaders.Authorization = new("Basic", $"{this.Configuration.ApiKey}:");

		this.ApiClient = new(this);
	}

	internal VimeoOttConfiguration Configuration { get; }

	internal HttpClient RestClient { get; }

	public bool CustomerBound { get; internal set; } = false;

	internal VimeoOttApiClient ApiClient { get; }
}
