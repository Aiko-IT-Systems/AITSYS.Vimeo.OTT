// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities;

/// <summary>
///     Represents a OTT HAL link.
/// </summary>
public sealed class OttHalLink
{
	/// <summary>
	///     Gets the href.
	/// </summary>
	[JsonProperty("href", NullValueHandling = NullValueHandling.Include)]
	public Uri? Href { get; internal set; }
}
