﻿// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using System.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace AITSYS.Vimeo.OTT.Entities;

/// <summary>
///     Represents the vimeo ott client configuration.
/// </summary>
public class VimeoOttConfiguration
{
	/// <summary>
	///     Sets the token used to identify the client (protected).
	/// </summary>
	private string? _apiKey;

	/// <summary>
	///     Creates a new configuration with default values.
	/// </summary>
	public VimeoOttConfiguration()
	{ }

	/// <summary>
	///     Utilized via dependency injection pipeline.
	/// </summary>
	/// <param name="provider">The service provider.</param>
	[ActivatorUtilitiesConstructor]
	public VimeoOttConfiguration(IServiceProvider provider)
	{
		this.ServiceProvider = provider;
	}

	/// <summary>
	///     Sets the customer href.
	/// </summary>
	public string? VhxCustomer { internal get; set; }

	/// <summary>
	///     Sets the customer client ip.
	/// </summary>
	public string? VhxClientIp { internal get; set; }

	/// <summary>
	///     Sets the token used to identify the client.
	/// </summary>
	public required string? ApiKey
	{
		internal get => this._apiKey;
		init
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException(nameof(value), "Api key cannot be null, empty, or all whitespace.");

			this._apiKey = value.Trim();
		}
	}

	/// <summary>
	///     <para>Sets the minimum logging level for messages.</para>
	///     <para>Defaults to <see cref="Information" />.</para>
	/// </summary>
	public LogLevel MinimumLogLevel { internal get; set; } = LogLevel.Information;

	/// <summary>
	///     <para>Allows you to overwrite the time format used by the internal debug logger.</para>
	///     <para>
	///         Only applicable when <see cref="LoggerFactory" /> is set left at default value. Defaults to ISO 8601-like
	///         format.
	///     </para>
	/// </summary>
	public string LogTimestampFormat { internal get; set; } = "yyyy-MM-dd HH:mm:ss zzz";

	/// <summary>
	///     <para>Sets the proxy to use for HTTP connections to VHX.</para>
	///     <para>Defaults to <see langword="null" />.</para>
	/// </summary>
	public IWebProxy? Proxy { internal get; set; } = null;

	/// <summary>
	///     <para>Sets the timeout for HTTP requests.</para>
	///     <para>Set to <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> to disable timeouts.</para>
	///     <para>Defaults to 20 seconds.</para>
	/// </summary>
	public TimeSpan HttpTimeout { internal get; set; } = TimeSpan.FromSeconds(20);

	/// <summary>
	///     <para>Sets the logger implementation to use.</para>
	///     <para>To create your own logger, implement the <see cref="Microsoft.Extensions.Logging.ILoggerFactory" /> instance.</para>
	///     <para>Defaults to built-in implementation.</para>
	/// </summary>
	public ILoggerFactory LoggerFactory { internal get; set; } = null!;

	/// <summary>
	///     <para>Sets the service provider.</para>
	///     <para>This allows passing data around without resorting to static members.</para>
	///     <para>Defaults to an empty service provider.</para>
	/// </summary>
	public IServiceProvider ServiceProvider { internal get; init; } = new ServiceCollection().BuildServiceProvider(true);
}