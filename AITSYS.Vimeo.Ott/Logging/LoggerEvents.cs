// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Microsoft.Extensions.Logging;

namespace AITSYS.Vimeo.Ott.Logging;

/// <summary>
///     Contains well-defined event IDs used by core of DisCatSharp.
/// </summary>
public static class LoggerEvents
{
	/// <summary>
	///     Miscellaneous events, that do not fit in any other category.
	/// </summary>
	public static EventId Misc { get; } = new(100, "VimeoOTT");

	/// <summary>
	///     Events pertaining to startup tasks.
	/// </summary>
	public static EventId Startup { get; } = new(101, nameof(Startup));

	/// <summary>
	///     Events emitted when exceptions are thrown in handlers attached to async events.
	/// </summary>
	public static EventId EventHandlerException { get; } = new(102, nameof(EventHandlerException));

	/// <summary>
	///     Events emitted when REST processing fails for any reason.
	/// </summary>
	public static EventId RestError { get; } = new(103, nameof(RestError));

	/// <summary>
	///     Events pertaining to ratelimit exhaustion.
	/// </summary>
	public static EventId RatelimitHit { get; } = new(104, nameof(RatelimitHit));

	/// <summary>
	///     Events pertaining to ratelimit diagnostics. Typically contain raw bucket info.
	/// </summary>
	public static EventId RatelimitDiag { get; } = new(105, nameof(RatelimitDiag));

	/// <summary>
	///     Events emitted when a ratelimit is exhausted and a request is preemptively blocked.
	/// </summary>
	public static EventId RatelimitPreemptive { get; } = new(106, nameof(RatelimitPreemptive));

	/// <summary>
	///     Events containing raw payloads, as they're received from Vimeo OTT's REST API.
	/// </summary>
	public static EventId RestRx { get; } = new(107, "REST ↓");

	/// <summary>
	///     Events containing raw payloads, as they're sent to Vimeo OTT's REST API.
	/// </summary>
	public static EventId RestTx { get; } = new(108, "REST ↑");

	/// <summary>
	///     Event is rest cleaner.
	/// </summary>
	public static EventId RestCleaner { get; } = new(109, nameof(RestCleaner));

	/// <summary>
	///     Event is rest hash mover.
	/// </summary>
	public static EventId RestHashMover { get; } = new(110, nameof(RestHashMover));

	/// <summary>
	///     Events is library side.
	/// </summary>
	public static EventId Library { get; } = new(111, nameof(Library));
}
