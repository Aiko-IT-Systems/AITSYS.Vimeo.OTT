// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents a free trail configuration.
/// </summary>
public sealed class OttFreeTrail
{
	/// <summary>
	///     Whether the free trail is enabled.
	/// </summary>
	[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool Enabled;
}
