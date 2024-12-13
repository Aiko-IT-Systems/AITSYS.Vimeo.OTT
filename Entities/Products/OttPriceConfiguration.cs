// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

/// <summary>
///     Represents a price configuration.
/// </summary>
public sealed class OttPriceConfiguration
{
	/// <summary>
	///     Gets the total cents.
	/// </summary>
	[JsonProperty("cents", NullValueHandling = NullValueHandling.Ignore)]
	public int Cents { get; internal set; }

	/// <summary>
	///     Gets the used currency.
	/// </summary>
	[JsonProperty("currency", NullValueHandling = NullValueHandling.Ignore)]
	public string Currency { get; internal set; }

	/// <summary>
	///     Gets the formatted price string.
	/// </summary>
	[JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
	public string Formatted { get; internal set; }

	/// <summary>
	///     Gets the max cents, only applicable for the pwyw config.
	/// </summary>
	[JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
	public int? Max { get; internal set; }
}
