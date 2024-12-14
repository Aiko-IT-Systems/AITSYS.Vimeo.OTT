// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Links;

public sealed class OttCustomerLinks : IHalLinks
{
	[JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Events { get; internal set; }

	[JsonProperty("watching", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Watching { get; internal set; }

	[JsonProperty("watchlist", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Watchlist { get; internal set; }

	/// <inheritdoc />
	[JsonProperty("self")]
	public OttHalLink Self { get; internal set; }
}
