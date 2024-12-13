// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Base;

/// <summary>
///     Represents geoblock settings.
/// </summary>
public sealed class OttGeoblock
{
	/// <summary>
	///     The affected countries.
	/// </summary>
	[JsonProperty("countries")]
	public List<string> Countries { get; internal set; } = [];

	/// <summary>
	///     The type of geoblock.
	///     Can be <c>allow</c> or <c>block</c>.
	/// </summary>
	[JsonProperty("type")]
	public string Type { get; internal set; }
}
