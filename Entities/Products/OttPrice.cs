// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents a products price.
/// </summary>
public sealed class OttPrice
{
	/// <summary>
	///     Gets the monthly price configuration.
	/// </summary>
	[JsonProperty("monthly", NullValueHandling = NullValueHandling.Ignore)]
	public OttPriceConfiguration Monthly { get; internal set; }

	/// <summary>
	///     Gets the yearly price configuration.
	/// </summary>
	[JsonProperty("yearly", NullValueHandling = NullValueHandling.Ignore)]
	public OttPriceConfiguration Yearly { get; internal set; }
}
