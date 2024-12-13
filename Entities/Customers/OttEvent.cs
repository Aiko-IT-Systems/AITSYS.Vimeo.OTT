// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;
using AITSYS.Vimeo.Ott.Entities.EmbeddedData;
using AITSYS.Vimeo.Ott.Entities.Links;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

public sealed class OttEvent : OttObject<OttEventLinks, OttEventEmbeddedData>
{
	/// <summary>
	///     The event topic.
	/// </summary>
	public string Topic { get; internal set; }

	/// <summary>
	///     The event data.
	/// </summary>
	public OttEventData Data { get; internal set; }

	/// <summary>
	///     Datetime when the event was created.
	/// </summary>
	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; internal set; }
}
