// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.EmbeddedData;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities;

/// <summary>
///     Represents a webhook event from Vimeo OTT.
/// </summary>
public sealed class OttWebhook
{
	/// <summary>
	///     Gets the embedded data related to the webhook event.
	/// </summary>
	[JsonProperty("_embedded", NullValueHandling = NullValueHandling.Ignore)]
	public OttCustomerProductEmbeddedData Embedded { get; set; }

	/// <summary>
	///     Gets the topic of the webhook event.
	/// </summary>
	[JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
	public string Topic { get; set; }

	/// <summary>
	///     Gets the date and time when the webhook event was created.
	/// </summary>
	[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
	public DateTime? CreatedAt { get; set; }
}
