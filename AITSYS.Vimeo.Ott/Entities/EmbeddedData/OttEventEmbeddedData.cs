// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Products;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.EmbeddedData;

public sealed class OttEventEmbeddedData : IOttEmbedded
{
	[JsonProperty("product", NullValueHandling = NullValueHandling.Ignore)]
	public string? Product { get; internal set; }
}