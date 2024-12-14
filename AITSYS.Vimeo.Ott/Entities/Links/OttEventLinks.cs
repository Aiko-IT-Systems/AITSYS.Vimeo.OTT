// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Links;

public sealed class OttEventLinks : IHalLinks
{
	[JsonProperty("product", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Product { get; internal set; }

	[JsonProperty("customer", NullValueHandling = NullValueHandling.Ignore)]
	public OttHalLink Customer { get; internal set; }

	/// <inheritdoc />
	public OttHalLink Self { get; internal set; } = new()
	{
		Href = null
	};
}
