// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Reflection;

using AITSYS.Vimeo.Ott.Entities;
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
}
