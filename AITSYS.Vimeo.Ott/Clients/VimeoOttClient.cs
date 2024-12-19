// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Reflection;

using AITSYS.Vimeo.Ott.Entities;
using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Pagination;
using AITSYS.Vimeo.Ott.Entities.Products;
using AITSYS.Vimeo.Ott.Logging;
using AITSYS.Vimeo.Ott.Rest;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Clients;

/// <summary>
///     Represents a client for Vimeo OTT.
/// </summary>
public sealed class VimeoOttClient
{
	/// <summary>
	///     Initializes a new instance of the <see cref="VimeoOttClient" /> class.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
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

	/// <summary>
	///     Gets the library version.
	/// </summary>
	public string LibraryVersion { get; }

	/// <summary>
	///     Gets the library name.
	/// </summary>
	public string LibraryName { get; }

	/// <summary>
	///     Gets the service provider.
	/// </summary>
	public IServiceProvider? ServiceProvider { get; }

	/// <summary>
	///     Gets the logger.
	/// </summary>
	public ILogger Logger { get; }

	/// <summary>
	///     Gets the configuration.
	/// </summary>
	internal VimeoOttConfiguration Configuration { get; }

	/// <summary>
	///     Gets a value indicating whether the client is bound to a customer.
	/// </summary>
	public bool CustomerBound { get; }

	/// <summary>
	///     Gets the API client.
	/// </summary>
	internal VimeoOttApiClient ApiClient { get; }

	/// <inheritdoc cref="VimeoOttApiClient.ExecuteRawRequestAsync" />
	public async Task<string> ExecuteRawRequestAsync(string url, RestRequestMethod method, string? payload = null)
		=> await this.ApiClient.ExecuteRawRequestAsync(url, method, payload);

	/// <inheritdoc cref="VimeoOttApiClient.ListCustomersAsync" />
	public async Task<OttPagination<OttCustomersEmbeddedData>> ListCustomersAsync(int? productId = null, string? email = null, string? query = null, string? sort = null, string? status = null, int page = 1, int perPage = 50)
		=> await this.ApiClient.ListCustomersAsync(productId, email, query, sort, status, page, perPage);

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveCustomerAsync" />
	public async Task<OttCustomer<OttCustomerProductEmbeddedData>?> RetrieveCustomerAsync(int customerId, int? productId = null)
		=> await this.ApiClient.RetrieveCustomerAsync(customerId, productId);

	/// <inheritdoc cref="VimeoOttApiClient.ListProductsAsync" />
	public async Task<OttPagination<OttProductsEmbeddedData>> ListProductsAsync(string? query = null, bool? active = null, string? sort = null, int page = 1, int perPage = 50)
		=> await this.ApiClient.ListProductsAsync(query, active, sort, page, perPage);

	/// <inheritdoc cref="VimeoOttApiClient.RetrieveProductAsync" />
	public async Task<OttProduct<OttProductEmbeddedData>?> RetrieveProductAsync(int productId)
		=> await this.ApiClient.RetrieveProductAsync(productId);
}
