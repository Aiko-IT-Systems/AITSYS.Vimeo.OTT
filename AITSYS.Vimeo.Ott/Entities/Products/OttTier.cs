// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

public sealed class OttTier
{
	[JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
	public int? Level { get; internal set; }

	[JsonProperty("free", NullValueHandling = NullValueHandling.Ignore)]
	public bool? Free { get; internal set; }
}
