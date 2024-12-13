﻿// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.EmbeddedData;

public class OttProductSectionsOnlyEmbeddedData : IOttEmbedded
{
	[JsonProperty("sections", NullValueHandling = NullValueHandling.Ignore)]
	public List<object> Sections { get; internal set; } = [];
}
