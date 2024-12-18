// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Logging;

/// <summary>
///     Represents a default logger provider.
/// </summary>
internal sealed class DefaultLoggerProvider : ILoggerProvider
{
	/// <summary>
	///     Gets the minimum log level.
	/// </summary>
	private readonly LogLevel _minimumLevel;

	/// <summary>
	///     Gets the timestamp format.
	/// </summary>
	private readonly string _timestampFormat;

	/// <summary>
	///     Gets a value indicating whether this <see cref="DefaultLoggerProvider" /> is disposed.
	/// </summary>
	private bool _isDisposed;

	/// <summary>
	///     Initializes a new instance of the <see cref="DefaultLoggerProvider" /> class.
	/// </summary>
	/// <param name="client">The client.</param>
	internal DefaultLoggerProvider(VimeoOttClient client)
		: this(client.Configuration.MinimumLogLevel, client.Configuration.LogTimestampFormat)
	{ }

	/// <summary>
	///     Initializes a new instance of the <see cref="DefaultLoggerProvider" /> class.
	/// </summary>
	/// <param name="minLevel">The min level.</param>
	/// <param name="timestampFormat">The timestamp format.</param>
	internal DefaultLoggerProvider(LogLevel minLevel = LogLevel.Information, string timestampFormat = "yyyy-MM-dd HH:mm:ss zzz")
	{
		this._minimumLevel = minLevel;
		this._timestampFormat = timestampFormat;
	}

	/// <summary>
	///     Creates the logger.
	/// </summary>
	/// <param name="categoryName">The category name.</param>
	public ILogger CreateLogger(string categoryName) =>
		this._isDisposed
			? throw new ObjectDisposedException(nameof(DefaultLoggerProvider), "This logger provider is already disposed.")
			: categoryName != typeof(VimeoOttClient).FullName
				? throw new ArgumentException($"This provider can only provide instances of loggers for {typeof(VimeoOttClient).FullName}", nameof(categoryName))
				: new DefaultLogger(this._minimumLevel, this._timestampFormat);

	/// <summary>
	///     Disposes the logger.
	/// </summary>
	public void Dispose()
		=> this._isDisposed = true;
}
