// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using AITSYS.Vimeo.Ott.Entities.Base;

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Entities.Customers;

public sealed class OttEventData
{
	[JsonProperty("action")]
	public string Action { get; internal set; }

	[JsonProperty("action_type")]
	public string? ActionType { get; internal set; }

	[JsonProperty("status")]
	public string? Status { get; internal set; }

	[JsonProperty("frequency")]
	public string? Frequency { get; internal set; }

	[JsonProperty("price")]
	public OttPrice? Price { get; internal set; }

	[JsonProperty("platform")]
	public string? Platform { get; internal set; }
}
