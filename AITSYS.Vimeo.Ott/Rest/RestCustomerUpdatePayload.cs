// Copyright 2025 Aiko IT Systems. See https://github.com/Aiko-IT-Systems/AITSYS.Vimeo.OTT/blob/main/LICENSE.md for the license.

using Newtonsoft.Json;

namespace AITSYS.Vimeo.Ott.Rest;

/// <summary>
///     Represents the payload for updating a customer.
/// </summary>
internal sealed class RestCustomerUpdatePayload
{
	/// <summary>
	///     Sets the customer's name.
	/// </summary>
	[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
	public string? Name { get; set; }

	/// <summary>
	///     Sets the customer's password.
	/// </summary>
	[JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
	public string? Password { get; set; }
}
