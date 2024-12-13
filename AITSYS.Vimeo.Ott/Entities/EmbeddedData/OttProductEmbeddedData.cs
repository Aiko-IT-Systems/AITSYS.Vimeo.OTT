// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.EmbeddedData;

public sealed class OttProductEmbeddedData : OttProductSectionsOnlyEmbeddedData
{
	[JsonProperty("trailer", NullValueHandling = NullValueHandling.Ignore)]
	public object? Trailer { get; internal set; }
}
