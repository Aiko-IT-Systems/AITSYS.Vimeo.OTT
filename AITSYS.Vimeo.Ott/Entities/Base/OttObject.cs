// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Base;

/// <summary>
///     Represents an api object within ott.
/// </summary>
/// <typeparam name="TOttHalLinks">The type of the HAL links.</typeparam>
/// <typeparam name="TOttEmbedded">The type of the embedded object.</typeparam>
public class OttObject<TOttHalLinks, TOttEmbedded> : IOttObject<TOttHalLinks, TOttEmbedded> where TOttHalLinks : IHalLinks where TOttEmbedded : IOttEmbedded
{
	/// <summary>
	///     Gets the HAL links.
	/// </summary>
	[JsonProperty("_links", NullValueHandling = NullValueHandling.Ignore)]
	public TOttHalLinks Links { get; internal set; }

	/// <summary>
	///     Gets the embedded data.
	/// </summary>
	[JsonProperty("_embedded", NullValueHandling = NullValueHandling.Ignore)]
	public TOttEmbedded Embedded { get; internal set; }
}
