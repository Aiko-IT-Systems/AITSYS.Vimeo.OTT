// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Links;

public sealed class OttProductLinks : IHalLinks
{
	[JsonProperty("browse_page", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink BrowsePage { get; internal set; }

	[JsonProperty("product_page", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink ProductPage { get; internal set; }

	[JsonProperty("customers", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Customers { get; internal set; }

	[JsonProperty("collections", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Collections { get; internal set; }

	[JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Series { get; internal set; }

	[JsonProperty("movies", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Movies { get; internal set; }

	[JsonProperty("playlists", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Playlists { get; internal set; }

	[JsonProperty("sections", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Sections { get; internal set; }

	[JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Categories { get; internal set; }

	/// <inheritdoc />
	[JsonProperty("self")]
	public OttHalLink Self { get; internal set; }
}
