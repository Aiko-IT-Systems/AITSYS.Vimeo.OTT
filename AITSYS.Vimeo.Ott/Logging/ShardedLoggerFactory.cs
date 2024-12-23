// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Logging;

/// <summary>
///     Represents a sharded logger factory.
/// </summary>
internal class ShardedLoggerFactory : ILoggerFactory
{
	/// <summary>
	///     Gets the logger.
	/// </summary>
	private readonly ILogger<VimeoOttClient> _logger;

	/// <summary>
	///     Initializes a new instance of the <see cref="ShardedLoggerFactory" /> class.
	/// </summary>
	/// <param name="instance">The instance.</param>
	public ShardedLoggerFactory(ILogger<VimeoOttClient> instance)
	{
		this._logger = instance;
	}

	/// <summary>
	///     Adds a provider.
	/// </summary>
	/// <param name="provider">The provider to be added.</param>
	public void AddProvider(ILoggerProvider provider)
		=> throw new InvalidOperationException("This is a passthrough logger container, it cannot register new providers.");

	/// <summary>
	///     Creates a logger.
	/// </summary>
	/// <param name="categoryName">The category name.</param>
	public ILogger CreateLogger(string categoryName) =>
		categoryName != typeof(VimeoOttClient).FullName
			? throw new ArgumentException($"This factory can only provide instances of loggers for {typeof(VimeoOttClient).FullName}, not for {categoryName}", nameof(categoryName))
			: this._logger;

	/// <summary>
	///     Disposes the logger.
	/// </summary>
	public void Dispose()
	{ }
}
