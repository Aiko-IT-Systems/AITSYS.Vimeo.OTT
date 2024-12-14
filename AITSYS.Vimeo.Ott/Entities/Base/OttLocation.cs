// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Base;

/// <summary>
///     Represents a customers location.
/// </summary>
public sealed class OttLocation
{
	/// <summary>
	///     The customers city.
	/// </summary>
	[JsonProperty("city")]
	public string City { get; internal set; }

	/// <summary>
	///     The customers region.
	/// </summary>
	[JsonProperty("region")]
	public string Region { get; internal set; }

	/// <summary>
	///     The customers country.
	/// </summary>
	[JsonProperty("country")]
	public string Country { get; internal set; }
}
