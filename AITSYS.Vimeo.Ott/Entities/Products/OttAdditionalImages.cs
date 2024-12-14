// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Products;

public sealed class OttAdditionalImages
{
	[JsonProperty("aspect_ratio_1_1", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio1X1 { get; internal set; }

	[JsonProperty("aspect_ratio_2_3", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio2X3 { get; internal set; }

	[JsonProperty("aspect_ratio_2_3_featured", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio2X3Featured { get; internal set; }

	[JsonProperty("aspect_ratio_12_5_logo", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio12X5Logo { get; internal set; }

	[JsonProperty("aspect_ratio_16_6", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio16X6 { get; internal set; }

	[JsonProperty("aspect_ratio_16_14", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio16X14 { get; internal set; }

	[JsonProperty("aspect_ratio_16_9_background", NullValueHandling = NullValueHandling.Ignore)]
	public OttImage? AspectRatio16X9Background { get; internal set; }
}
