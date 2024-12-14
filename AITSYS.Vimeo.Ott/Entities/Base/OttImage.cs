// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Base;

public sealed class OttImage
{
	[JsonProperty("blurred", NullValueHandling = NullValueHandling.Ignore)]
	public Uri Blurred;

	[JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
	public Uri Large;

	[JsonProperty("medium", NullValueHandling = NullValueHandling.Ignore)]
	public Uri Medium;

	[JsonProperty("small", NullValueHandling = NullValueHandling.Ignore)]
	public Uri Small;

	[JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
	public Uri Source;
}
