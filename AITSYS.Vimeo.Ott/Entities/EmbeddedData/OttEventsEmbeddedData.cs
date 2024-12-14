// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Customers;
using AITSYS.Vimeo.Ott.Interfaces;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.EmbeddedData;

public sealed class OttEventsEmbeddedData<TOttEmbedded> : IOttEmbedded where TOttEmbedded : IOttEmbedded
{
	/// <summary>
	///     Gets the events.
	/// </summary>
	[JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
	public List<OttEvent<TOttEmbedded>> Events { get; internal set; } = [];
}
