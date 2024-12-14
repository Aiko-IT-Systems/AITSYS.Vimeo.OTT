// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Clients;

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Logging;

/// <summary>
///     Represents a composite default logger.
/// </summary>
internal class CompositeDefaultLogger : ILogger<VimeoOttClient>
{
	/// <summary>
	///     Gets the loggers.
	/// </summary>
	private readonly List<ILogger<VimeoOttClient>> _loggers;

	/// <summary>
	///     Initializes a new instance of the <see cref="CompositeDefaultLogger" /> class.
	/// </summary>
	/// <param name="providers">The providers.</param>
	public CompositeDefaultLogger(IEnumerable<ILoggerProvider> providers)
	{
		this._loggers = providers.Select(x => x.CreateLogger(typeof(VimeoOttClient).FullName!))
			.OfType<ILogger<VimeoOttClient>>()
			.ToList();
	}

	/// <summary>
	///     Whether the logger is enabled.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	public bool IsEnabled(LogLevel logLevel)
		=> true;

	/// <summary>
	///     Logs an event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="eventId">The event id.</param>
	/// <param name="state">The state.</param>
	/// <param name="exception">The exception.</param>
	/// <param name="formatter">The formatter.</param>
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		foreach (var logger in this._loggers)
			logger.Log(logLevel, eventId, state, exception, formatter);
	}

	/// <summary>
	///     Begins the scope.
	/// </summary>
	/// <param name="state">The state.</param>
	public IDisposable BeginScope<TState>(TState state) where TState : notnull
		=> throw new NotImplementedException();
}
