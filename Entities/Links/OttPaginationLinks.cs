// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Links;

public sealed class OttPaginationLinks : IHalLinks
{
	[JsonProperty("first", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink First { get; internal set; }

	[JsonProperty("prev", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink? Prev { get; internal set; }

	[JsonProperty("next", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink? Next { get; internal set; }

	[JsonProperty("last", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Last { get; internal set; }

	/// <inheritdoc />
	[JsonProperty("self")]
	public OttHalLink Self { get; internal set; }
}
