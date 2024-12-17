// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Reflection;

using AITSYS.Vimeo.Ott.Entities;
using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Entities.Products;
using AITSYS.Vimeo.Ott.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Clients;

public sealed class VimeoOttClient
{
	public VimeoOttClient(VimeoOttConfiguration configuration)
	{
		this.Configuration = configuration;
		this.ServiceProvider = this.Configuration.ServiceProvider;

		if (this.ServiceProvider is not null)
		{
			this.Configuration.LoggerFactory ??= this.Configuration.ServiceProvider.GetService<ILoggerFactory>()!;
			this.Logger = this.Configuration.ServiceProvider.GetService<ILogger<VimeoOttClient>>()!;
		}

		if (this.Configuration.LoggerFactory is null)
		{
			this.Configuration.LoggerFactory = new DefaultLoggerFactory();
			this.Configuration.LoggerFactory.AddProvider(new DefaultLoggerProvider(this));
		}

		this.Logger ??= this.Configuration.LoggerFactory.CreateLogger<VimeoOttClient>();

		this.Logger.LogInformation(LoggerEvents.Library, "Setting up VHX client");

		var a = typeof(VimeoOttClient).GetTypeInfo().Assembly;
		var iv = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		if (iv != null)
			this.LibraryVersion = iv.InformationalVersion;
		else
		{
			var v = a.GetName().Version;
			if (v is null)
				this.LibraryVersion = "0.0.1";
			else
			{
				var vs = v.ToString(3);

				this.LibraryVersion = v.Revision > 0
					? $"{vs}, CI build {v.Revision}"
					: vs;
			}
		}

		this.LibraryName = nameof(VimeoOttClient);

		if (this.Configuration.VhxCustomer is not null)
			this.CustomerBound = true;

		this.ApiClient = new(this);

		this.Logger.LogInformation(LoggerEvents.Library, "{bound}VHX client is ready", this.CustomerBound ? "Customer bound " : "");
	}

	public string LibraryVersion { get; }

	public string LibraryName { get; }

	public IServiceProvider? ServiceProvider { get; }

	public ILogger Logger { get; }

	internal VimeoOttConfiguration Configuration { get; }

	public bool CustomerBound { get; }

	internal VimeoOttApiClient ApiClient { get; }

	/// <inheritdoc cref="VimeoOttApiClient.ListCustomersAsync" />
	public async Task<OttPagination<OttCustomersEmbeddedData>> ListCustomersAsync(string? productId = null, string? email = null, string? query = null, string? sort = null, string? status = null, int page = 1, int perPage = 50)
		=> await this.ApiClient.ListCustomersAsync(productId, email, query, sort, status, page, perPage);

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveCustomerAsync" />
	public async Task<OttCustomer<OttCustomerProductEmbeddedData>> RetrieveCustomerAsync(int customerId, int? productId = null)
		=> await this.ApiClient.RetrieveCustomerAsync(customerId, productId);

	/// <inheritdoc cref="VimeoOttApiClient.ListProductsAsync" />
	public async Task<OttPagination<OttProductsEmbeddedData>> ListProductsAsync(string? query = null, bool? active = null, string? sort = null, int page = 1, int perPage = 50)
		=> await this.ApiClient.ListProductsAsync(query, active, sort, page, perPage);

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveProductAsync" />
	public async Task<OttProduct<OttProductEmbeddedData>> RetrieveProductAsync(int productId)
		=> await this.ApiClient.RetrieveProductAsync(productId);
}
