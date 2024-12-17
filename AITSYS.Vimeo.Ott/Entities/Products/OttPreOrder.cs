// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents the pre order config.
/// </summary>
public sealed class OttPreOrder
{
	/// <summary>
	///     Gets whether pre ordering is enabled.
	/// </summary>
	[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
	public bool Enabled { get; internal set; }

	/// <summary>
	///     Gets the planned release date.
	/// </summary>
	[JsonProperty("release_date", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? ReleaseData { get; internal set; }
}
